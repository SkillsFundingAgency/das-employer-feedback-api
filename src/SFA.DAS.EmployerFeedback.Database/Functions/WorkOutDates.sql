CREATE FUNCTION [dbo].[WorkOutDates]
(
    @calcdate datetime,
    @reset int = 0
)
RETURNS
@dates TABLE
(
    timelimit varchar(6),
    startdate date,
    enddate date,
    limit5AY varchar(6),
    limit1AY varchar(6),
    calcdate date
)
AS
BEGIN
-- calcuates the date ranges for Feedback Summaries

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
        
    INSERT @dates
    VALUES(@timelimit,    @startdate,  @enddate,  @limit5AY, @limit1AY, @calcdate);

RETURN;
END;