-- =========================================
-- Suppression des données existantes
-- =========================================
USE HRMS;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
    
-- Désactiver les contraintes FK temporairement
ALTER TABLE Departments NOCHECK CONSTRAINT FK_Departments_Employees_ManagerId;
ALTER TABLE Employees NOCHECK CONSTRAINT FK_Employees_Departments_DepartmentId;
ALTER TABLE Employees NOCHECK CONSTRAINT FK_Employees_Positions_PositionId;

-- Supprimer les données
DELETE FROM Promotions;
DELETE FROM Interviews;
DELETE FROM Candidates;
DELETE FROM JobOffers;
DELETE FROM Documents;
DELETE FROM Bonuses;
DELETE FROM Salaries;
DELETE FROM EmployeeBenefits;
DELETE FROM EquipmentAssignments;
DELETE FROM Users;
DELETE FROM Employees;
DELETE FROM Benefits;
DELETE FROM Equipments;
DELETE FROM Positions;
DELETE FROM Departments;

-- Réactiver les contraintes FK
ALTER TABLE Departments CHECK CONSTRAINT FK_Departments_Employees_ManagerId;
ALTER TABLE Employees CHECK CONSTRAINT FK_Employees_Departments_DepartmentId;
ALTER TABLE Employees CHECK CONSTRAINT FK_Employees_Positions_PositionId;

-- Departments
SET IDENTITY_INSERT Departments ON;
INSERT INTO Departments (DepartmentId, Name, Code, Description, ManagerId, CreatedDate, ModifiedDate)
VALUES
    (1, 'Informatique', 'IT', 'Département informatique', NULL, GETDATE(), GETDATE()),
    (2, 'Ressources Humaines', 'RH', 'Département RH', NULL, GETDATE(), GETDATE());
SET IDENTITY_INSERT Departments OFF;
    

-- Positions
SET IDENTITY_INSERT Positions ON;
INSERT INTO Positions (PositionId, Title, Description, BaseSalary, CreatedDate, ModifiedDate)
VALUES
    (1, 'Développeur', 'Développeur .NET', 3000, GETDATE(), GETDATE()),
    (2, 'Manager IT', 'Responsable informatique', 5000, GETDATE(), GETDATE()),
    (3, 'RH', 'Responsable RH', 4500, GETDATE(), GETDATE());
SET IDENTITY_INSERT Positions OFF;


-- Employees
SET IDENTITY_INSERT Employees ON;
INSERT INTO Employees (EmployeeId, Matricule, FirstName, LastName, DateOfBirth, Address, Phone, Email, DepartmentId, PositionId, HireDate, ContractType, Status, CreatedDate, ModifiedDate)
VALUES
    (1, 'EMP001', 'Mohamed', 'Khalil', '1985-06-15', 'Tunis', '12345678', 'mohamed.khalil@hrms.tn', 1, 2, '2010-01-01', 'CDI', 0, GETDATE(), GETDATE()),
    (2, 'EMP002', 'Sami', 'Bouaziz', '1990-03-20', 'Sousse', '87654321', 'sami.bouaziz@hrms.tn', 2, 3, '2015-05-01', 'CDI', 0, GETDATE(), GETDATE());
SET IDENTITY_INSERT Employees OFF;


-- Users
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (UserId, Username, PasswordHash, Email, EmployeeId, Role, IsActive, CreatedDate, ModifiedDate)
VALUES
    (1, 'admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'admin@hrms.tn', 2, 0, 1, GETDATE(), GETDATE()),
    (2, 'manager.it', 'd74ff0ee8da3b9806b18c877dbf29bbde50b5bd8e4dad7a3a725000feb82e8f1', 'mohamed.khalil@hrms.tn', 1, 1, 1, GETDATE(), GETDATE());
SET IDENTITY_INSERT Users OFF;

-- Benefits
SET IDENTITY_INSERT Benefits ON;
INSERT INTO Benefits (BenefitId, BenefitType, Description, Value, CreatedDate, ModifiedDate)
VALUES
    (1, 'Assurance Santé', 'Assurance médicale pour lemployé', 200, GETDATE(), GETDATE()),
(2, 'Ticket Restaurant', 'Ticket restaurant mensuel', 100, GETDATE(), GETDATE());
SET IDENTITY_INSERT Benefits OFF;

-- EmployeeBenefits
SET IDENTITY_INSERT EmployeeBenefits ON;
INSERT INTO EmployeeBenefits (EmployeeBenefitId, EmployeeId, BenefitId, StartDate, EndDate, CreatedDate)
VALUES
(1, 1, 1, GETDATE(), NULL, GETDATE()),
(2, 2, 2, GETDATE(), NULL, GETDATE());
SET IDENTITY_INSERT EmployeeBenefits OFF;


-- Equipment
SET IDENTITY_INSERT Equipments ON;    
INSERT INTO Equipments (EquipmentId, EquipmentType, SerialNumber, Brand, Model, PurchaseDate, Status, CreatedDate, ModifiedDate)
VALUES
(1, 'Ordinateur Portable', 'SN12345', 'Dell', 'Latitude', '2022-01-01', 1, GETDATE(), GETDATE()),
(2, 'Téléphone', 'SN54321', 'Samsung', 'Galaxy', '2023-01-01', 1, GETDATE(), GETDATE());
SET IDENTITY_INSERT Equipments OFF;
    
-- EquipmentAssignments
SET IDENTITY_INSERT EquipmentAssignments ON;
INSERT INTO EquipmentAssignments (AssignmentId, EquipmentId, EmployeeId, AssignmentDate, ReturnDate, Condition, Notes, CreatedDate)
VALUES
(1, 1, 1, GETDATE(), NULL, 'Neuf', 'Aucune remarque', GETDATE()),
(2, 2, 2, GETDATE(), NULL, 'Neuf', 'Aucune remarque', GETDATE());
SET IDENTITY_INSERT EquipmentAssignments OFF;    

-- Salaries
SET IDENTITY_INSERT Salaries ON;
INSERT INTO Salaries (SalaryId, EmployeeId, BaseSalary, EffectiveDate, EndDate, Justification, CreatedDate)
VALUES
(1, 1, 5000, GETDATE(), NULL, 'Salaire initial', GETDATE()),
(2, 2, 4500, GETDATE(), NULL, 'Salaire initial', GETDATE());
SET IDENTITY_INSERT Salaries OFF;    

-- Bonuses
SET IDENTITY_INSERT Bonuses ON;
INSERT INTO Bonuses (BonusId, EmployeeId, BonusType, Amount, Date, Description, CreatedDate)
VALUES
(1, 1, 'Prime performance', 500, GETDATE(), 'Bonus annuel', GETDATE());
SET IDENTITY_INSERT Bonuses OFF;    


-- Documents
SET IDENTITY_INSERT Documents ON;
INSERT INTO Documents (DocumentId, EmployeeId, DocumentType, FileName, FilePath, UploadDate)
VALUES
(1, 1, 'CV', 'CV_Mohamed.pdf', '/docs/cv_mohamed.pdf', GETDATE()),
(2, 2, 'CV', 'CV_Sami.pdf', '/docs/cv_sami.pdf', GETDATE());
SET IDENTITY_INSERT Documents OFF;
    
-- JobOffers
SET IDENTITY_INSERT JobOffers ON;
INSERT INTO JobOffers (JobOfferId, Title, Description, DepartmentId, PositionId, PostDate, ExpiryDate, Status, CreatedDate, ModifiedDate)
VALUES
(1, 'Développeur Junior', 'Poste à pourvoir', 1, 1, GETDATE(), DATEADD(month, 1, GETDATE()), 0, GETDATE(), GETDATE());
SET IDENTITY_INSERT JobOffers OFF;

-- Candidates
SET IDENTITY_INSERT Candidates ON;
INSERT INTO Candidates (CandidateId, FirstName, LastName, Email, Phone, CVPath, JobOfferId, Status, ApplicationDate)
VALUES
(1, 'Ali', 'Ben Ahmed', 'ali.benahmed@example.com', '11122233', '/cv/ali.pdf', 1, 1, GETDATE());
SET IDENTITY_INSERT Candidates OFF;
    

-- Interviews
SET IDENTITY_INSERT Interviews ON;
INSERT INTO Interviews (InterviewId, CandidateId, InterviewDate, Location, InterviewerName, Notes, Result, CreatedDate)
VALUES
(1, 1, GETDATE(), 'Salle 101', 'Mohamed Khalil', 'Bonne présentation', 0, GETDATE());
SET IDENTITY_INSERT Interviews OFF;

-- Promotions
SET IDENTITY_INSERT Promotions ON;
INSERT INTO Promotions (PromotionId, EmployeeId, OldPositionId, NewPositionId, OldSalary, NewSalary, PromotionDate, Justification, CreatedDate)
VALUES
(1, 2, 3, 2, 4500, 5000, GETDATE(), 'Promotion interne', GETDATE());
SET IDENTITY_INSERT Promotions OFF;