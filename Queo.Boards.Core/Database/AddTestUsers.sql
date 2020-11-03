SET IDENTITY_INSERT tblUser ON;


insert into tblUser 
	(Id, BusinessId, UserName   , Firstname, Lastname, Company, PasswordHash                                                          , UserCategory, IsEnabled)
values
	(1 , NEWID()   , 'testadmin', 'Test'   , 'Admin' , 'queo' , 'AEn7weSEHpyLzQpUlVDatkZhd9NK8jtZsp2sJ4FOzb5KC+uiF40a5SvKWj6gMKzRiw==', 'Local', 1 ),
	(2 , NEWID()   , 'testuser',  'Test'   , 'User'  , 'queo' , 'AE8UUr9tcAEWLCQWiTgAAg9SVw0ufFyOhpnyOYDGZpPtOrUbyugvmRcypfg2VQFQtA==', 'Local', 1 );

Insert into tblUserRoles 
VALUES 
	(1, 'Administrator'),
	(2, 'User')
;

SET IDENTITY_INSERT tblUser OFF;