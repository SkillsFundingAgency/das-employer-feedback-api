-- Employer Feedback
-- NOTE only aggregates for the current AY unless is reset, in which case will redo 5 years

CREATE PROCEDURE [dbo].[GenerateProviderRatingResults]
(
    @execdate datetime = NULL,
    @reset int = 0  -- set to 1 to do a full reset
 )
AS
BEGIN
    DECLARE 
    @oldTolerance FLOAT = 0.3,  -- Tolerance for years before AY2425
    @newTolerance FLOAT = 0.5;  -- Tolerance for years AY2425 and beyond

    -- Set date ranges and limit to 5 years from calcdate (default now)
    DECLARE @limit5AY varchar(6), @limit1AY varchar(6), @timelimit varchar(6), @startdate date, @enddate date, @calcdate date;
    
	SELECT @timelimit=timelimit, @startdate=startdate, @enddate=enddate, @limit5AY =limit5AY, @limit1AY =limit1AY, @calcdate = calcdate
	FROM [dbo].[WorkOutDates] (@execdate, @reset);

    WITH LatestRatings 
    AS (
        SELECT er1.FeedbackId, er1.ProviderRating, eft.Ukprn, TimePeriod
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
    )
        
    -- Get the ratings for required AY results for each UKPRN (not 'All')
    MERGE INTO [dbo].[ProviderRatingSummary] prs 
    USING (  

        -- Year-on-Year Results
        SELECT TimePeriod, Ukprn
              ,ProviderRating Rating 
              ,COUNT(*) RatingCount
        FROM LatestRatings
        GROUP BY TimePeriod, Ukprn, ProviderRating
     ) upd
    ON prs.Ukprn = upd.Ukprn AND prs.Rating = upd.Rating AND prs.TimePeriod = upd.TimePeriod
    WHEN MATCHED THEN 
        UPDATE SET prs.RatingCount = upd.RatingCount, 
                   prs.UpdatedOn = @calcdate
    WHEN NOT MATCHED BY TARGET THEN 
        INSERT (Ukprn, Rating, RatingCount, UpdatedOn, TimePeriod)
        VALUES (upd.Ukprn, upd.Rating, upd.RatingCount, @calcdate, upd.TimePeriod)
    WHEN NOT MATCHED BY SOURCE AND prs.TimePeriod BETWEEN @limit1AY AND @timelimit THEN
        DELETE;


    -- Get the Stars for all eligible 5 Year results for each UKPRN
    WITH av1 
    AS(
        SELECT TimePeriod, Ukprn, ReviewCount,
            CASE
            -- handle change of logic from Aug 2024
            WHEN TimePeriod < 'AY2425'
            THEN
              (CASE
               WHEN AvgRating >= 3.0 + @oldTolerance THEN 4
               WHEN AvgRating >= 2.0 + @oldTolerance THEN 3
               WHEN AvgRating >= 1.0 + @oldTolerance THEN 2
               ELSE 1 END)
            ELSE
              (CASE
               WHEN AvgRating >= 3.0 + @newTolerance THEN 4
               WHEN AvgRating >= 2.0 + @newTolerance THEN 3
               WHEN AvgRating >= 1.0 + @newTolerance THEN 2
               ELSE 1 END) 
            END Stars
        FROM (
            SELECT TimePeriod, Ukprn, SUM(RatingCount) ReviewCount,
                ROUND(CAST(SUM((CASE [Rating] WHEN 'VeryPoor' THEN 1 WHEN 'Poor' THEN 2 WHEN 'Good' THEN 3 WHEN 'Excellent' THEN 4 ELSE 1 END) * RatingCount) AS FLOAT) / CAST(SUM(RatingCount) AS FLOAT), 1) AvgRating
            FROM [dbo].[ProviderRatingSummary]
            GROUP BY TimePeriod, Ukprn
        ) ab2
    )

    MERGE INTO [dbo].[ProviderStarsSummary] pss
    USING (
        SELECT TimePeriod, Ukprn, ReviewCount, Stars
        FROM av1
        UNION ALL
        -- All, adding each year's results
        SELECT 'All' TimePeriod, Ukprn
               ,SUM(ReviewCount) ReviewCount 
               ,ROUND(AVG(CAST(Stars AS FLOAT)), 0) Stars
        FROM av1 
        WHERE TimePeriod >= @limit5AY  -- will ignore 'All'
        GROUP BY Ukprn        
    ) upd
    ON pss.Ukprn = upd.Ukprn AND pss.TimePeriod = upd.TimePeriod
    WHEN MATCHED THEN
        UPDATE SET pss.ReviewCount = upd.ReviewCount,
                   pss.Stars = upd.Stars
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (Ukprn, ReviewCount, Stars, TimePeriod)
        VALUES (upd.Ukprn, upd.ReviewCount, upd.Stars, upd.TimePeriod)
    WHEN NOT MATCHED BY SOURCE THEN
        DELETE;  
          
END
GO
