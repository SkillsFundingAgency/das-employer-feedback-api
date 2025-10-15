CREATE TABLE FeedbackTransaction (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,                        
    AccountId BIGINT NOT NULL,                     
    TemplateName VARCHAR(255) NOT NULL,            
    SendAfter DATETIME NOT NULL,                   
    TemplateId UNIQUEIDENTIFIER, 
    SentCount INT NULL,
    SentDate DATETIME NULL,
    CreatedOn DATETIME NOT NULL,

    CONSTRAINT FK_FeedbackTransaction_Account FOREIGN KEY (AccountId)
        REFERENCES Account(Id)
);