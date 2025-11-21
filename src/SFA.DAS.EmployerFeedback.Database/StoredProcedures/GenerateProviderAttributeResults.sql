-- Employer Feedback
-- NOTE only aggregates for the current AY unless is reset, in which case will redo 5 years
CREATE PROCEDURE [dbo].[GenerateProviderAttributeResults]
(
    @calcdate datetime = NULL,
    @reset int = 0  -- set to 1 to do a full reset
)
AS

BEGIN
    IF @calcdate IS NULL
    -- Default is now, but can be overriden for testing / back dating
        SET @calcdate = GETUTCDATE();
        
    -- Set limit to 5 years
    DECLARE 
    @timelimit varchar(6),
    @startdate date = CONVERT(date,CONVERT(varchar,YEAR(DATEADD(month,-7,@calcdate)))+'-Aug-01'),
    @enddate date =   CONVERT(date,CONVERT(varchar,YEAR(DATEADD(month,5,@calcdate)))+'-Aug-01'),
    @limit5AY varchar(6) = 'AY'+RIGHT(YEAR(DATEADD(month,-55,@calcdate)),2)+RIGHT(YEAR(DATEADD(month,-43,@calcdate)),2),
    @limit1AY varchar(6) = 'AY'+RIGHT(YEAR(DATEADD(month,-7,@calcdate)),2)+RIGHT(YEAR(DATEADD(month,5,@calcdate)),2);
    
    IF @reset = 1
    -- reset all 5 AYs
    BEGIN
        SET @timelimit = @limit5AY;
        SET @startdate = DATEADD(year,-4,@startdate);
    END
    ELSE
        SET @timelimit = @limit1AY;

    WITH LatestResults 
    AS (
        SELECT er1.FeedbackId, pa1.AttributeId, pa1.AttributeValue, eft.Ukprn, TimePeriod
        FROM (
          -- get latest feedback for each feedback target
            SELECT * FROM (
                SELECT ROW_NUMBER() OVER (PARTITION BY TimePeriod,FeedbackId ORDER BY DateTimeCompleted DESC) seq, *
                FROM (
                    SELECT *
                          ,'AY'+RIGHT(YEAR(DATEADD(month,-7,DateTimeCompleted)),2)+RIGHT(YEAR(DATEADD(month,5,DateTimeCompleted)),2) TimePeriod
                    FROM [dbo].[EmployerFeedbackResult]
                    WHERE (@reset =1 OR (DateTimeCompleted >= @startdate AND DateTimeCompleted < @enddate))
                ) er3
            ) er2 
            -- handle change of logic from Aug 2024
            WHERE TimePeriod < 'AY2425' OR seq = 1 
        ) er1
        JOIN [dbo].[EmployerFeedback] eft on er1.FeedbackId = eft.FeedbackId
        JOIN [dbo].[ProviderAttributes] pa1 on pa1.EmployerFeedbackResultId = er1.Id
    )
    -- Get the ratings for required AY results for each UKPRN
    MERGE INTO [dbo].[ProviderAttributeSummary] pas 
    USING (  

        -- Year-on-Year Results
        SELECT TimePeriod, Ukprn, AttributeId
              ,SUM(CASE WHEN AttributeValue = 1 THEN 1 ELSE 0 END) Strength 
              ,SUM(CASE WHEN AttributeValue = 1 THEN 0 ELSE 1 END) Weakness
        FROM LatestResults
        GROUP BY TimePeriod, Ukprn, AttributeId

     ) upd
    ON pas.Ukprn = upd.Ukprn AND pas.AttributeId = upd.AttributeId AND pas.TimePeriod = upd.TimePeriod
    WHEN MATCHED THEN 
        UPDATE SET pas.Strength = upd.Strength, 
                   pas.Weakness = upd.Weakness,
                   pas.UpdatedOn = @calcdate
    WHEN NOT MATCHED BY TARGET THEN 
        INSERT (Ukprn, AttributeId, Strength, Weakness, UpdatedOn, TimePeriod) 
        VALUES (upd.Ukprn, upd.AttributeId, upd.Strength, upd.Weakness, @calcdate, upd.TimePeriod)
    WHEN NOT MATCHED BY SOURCE AND TimePeriod BETWEEN @limit1AY AND @timelimit THEN
        DELETE;

    -- Get the ratings for all eligible 5 Year results for each UKPRN
    MERGE INTO [dbo].[ProviderAttributeSummary] pas 
    USING (  
        -- All, adding each year's results
        SELECT 'All' TimePeriod, Ukprn, AttributeId
               ,SUM(Strength) Strength 
               ,SUM(Weakness) Weakness 
        FROM [dbo].[ProviderAttributeSummary] 
        WHERE TimePeriod >= @limit5AY  -- will ignore 'All'
        GROUP BY Ukprn, AttributeId
     ) upd
    ON pas.Ukprn = upd.Ukprn AND pas.AttributeId = upd.AttributeId AND pas.TimePeriod = upd.TimePeriod
    WHEN MATCHED THEN 
        UPDATE SET pas.Strength = upd.Strength, 
                   pas.Weakness = upd.Weakness,
                   pas.UpdatedOn = @calcdate
    WHEN NOT MATCHED BY TARGET THEN 
        INSERT (Ukprn, AttributeId, Strength, Weakness, UpdatedOn, TimePeriod) 
        VALUES (upd.Ukprn, upd.AttributeId, upd.Strength, upd.Weakness, @calcdate, upd.TimePeriod)
    WHEN NOT MATCHED BY SOURCE AND TimePeriod = 'All' THEN
        DELETE;
    
  
END
GO
