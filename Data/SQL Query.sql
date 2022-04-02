
CREATE TABLE tblRole (
    RoleId int IDENTITY(1,1),
    Role nvarchar(255),
    CreatedBy int,
    CreatedDate datetime,
    EditBy int,
    EditDate datetime,
	DeletedBy int,
    isActive bit,
	PRIMARY KEY (RoleId)
);

CREATE TABLE tblUser (
    UserId int IDENTITY(1,1),
    Name nvarchar(255),
    Email nvarchar(255),
    Password nvarchar(255),
    RoleId int,
    CreatedBy int,
    CreatedDate datetime,
    EditBy int,
    EditDate datetime,
    DeletedBy int,
    isActive bit,
	PRIMARY KEY (UserId),
    FOREIGN KEY (RoleId) REFERENCES tblRole(RoleId),
    FOREIGN KEY (CreatedBy) REFERENCES tblUser(UserId),
    FOREIGN KEY (EditBy) REFERENCES tblUser(UserId),
    FOREIGN KEY (DeletedBy) REFERENCES tblUser(UserId)
);

ALTER TABLE tblRole
ADD FOREIGN KEY (CreatedBy) REFERENCES tblUser(UserId),
    FOREIGN KEY (EditBy) REFERENCES tblUser(UserId),
    FOREIGN KEY (DeletedBy) REFERENCES tblUser(UserId);

CREATE TABLE tblMenu (
    MenuId int IDENTITY(1,1),
    Name nvarchar(255),
    ControllerName nvarchar(255),
    ActionName nvarchar(255),
    isParent bit,
    ParentId int,
    FontAwesome nvarchar(255),
    CreatedBy int,
    CreatedDate datetime,
    EditBy int,
    EditDate datetime,
    DeletedBy int,
    isActive bit,
    ElementId nvarchar(255),
	PRIMARY KEY (MenuId),
    FOREIGN KEY (ParentId) REFERENCES tblMenu(MenuId),
    FOREIGN KEY (CreatedBy) REFERENCES tblUser(UserId),
    FOREIGN KEY (EditBy) REFERENCES tblUser(UserId),
    FOREIGN KEY (DeletedBy) REFERENCES tblUser(UserId)
);


CREATE TABLE tblAccessLevel (
    AccessLevelId int IDENTITY(1,1),
    MenuId int,
    RoleId int,
    CreateAccess bit,
    EditAccess bit,
    DeleteAccess bit,
    CreatedBy int,
    CreatedDate datetime,
    EditBy int,
    EditDate datetime,
    DeletedBy int,
    isActive bit,
	PRIMARY KEY (AccessLevelId),
    FOREIGN KEY (MenuId) REFERENCES tblMenu(MenuId),
    FOREIGN KEY (RoleId) REFERENCES tblRole(RoleId),
    FOREIGN KEY (CreatedBy) REFERENCES tblUser(UserId),
    FOREIGN KEY (EditBy) REFERENCES tblUser(UserId),
    FOREIGN KEY (DeletedBy) REFERENCES tblUser(UserId)
);

CREATE TABLE tblArticle (
    ArticleId int IDENTITY(1,1),
    Author int,
    Title nvarchar(255),
    Status int,
    Body nvarchar(max),
    CreatedBy int,
    CreatedDate datetime,
    EditBy int,
    EditDate datetime,
    DeletedBy int,
    isActive bit,
	PRIMARY KEY (ArticleId),
    FOREIGN KEY (Author) REFERENCES tblUser(UserId),
    FOREIGN KEY (CreatedBy) REFERENCES tblUser(UserId),
    FOREIGN KEY (EditBy) REFERENCES tblUser(UserId),
    FOREIGN KEY (DeletedBy) REFERENCES tblUser(UserId)
);