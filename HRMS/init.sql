-- =============================================
-- Script d'Initialisation avec Données de Test
-- SGRH - Système de Gestion des Ressources Humaines
-- =============================================

USE HRMS;
GO

DELETE FROM Interviews;
DELETE FROM Candidates;
DELETE FROM JobOffers;
DELETE FROM Promotions;
DELETE FROM EquipmentAssignments;
DELETE FROM Equipments;
DELETE FROM EmployeeBenefits;
DELETE FROM Benefits;
DELETE FROM Bonuses;
DELETE FROM Salaries;
DELETE FROM Users;
DELETE FROM Employees;
DELETE FROM Positions;
DELETE FROM Departments;

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;

-- =============================================
-- 1. Départements
-- =============================================
INSERT INTO Departments (Name, Code, Description, CreatedDate, ModifiedDate)
VALUES 
('Direction Générale', 'DG', 'Direction générale et stratégique', GETDATE(), GETDATE()),
('Ressources Humaines', 'RH', 'Gestion des ressources humaines', GETDATE(), GETDATE()),
('Informatique', 'IT', 'Département informatique et technique', GETDATE(), GETDATE()),
('Commercial', 'COM', 'Équipe commerciale et ventes', GETDATE(), GETDATE()),
('Finance', 'FIN', 'Comptabilité et finances', GETDATE(), GETDATE()),
('Marketing', 'MKT', 'Marketing et communication', GETDATE(), GETDATE()),
('Production', 'PROD', 'Production et opérations', GETDATE(), GETDATE());

-- =============================================
-- 2. Postes
-- =============================================
INSERT INTO Positions (Title, Description, BaseSalary, CreatedDate, ModifiedDate)
VALUES
    ('Directeur Général', 'Responsable de la direction générale', 8000.00, GETDATE(), GETDATE()),
    ('Directeur RH', 'Responsable des ressources humaines', 5500.00, GETDATE(), GETDATE()),
    ('Chef de Projet IT', 'Gestion de projets informatiques', 4500.00, GETDATE(), GETDATE()),
    ('Développeur Senior', 'Développement et maintenance applications', 3500.00, GETDATE(), GETDATE()),
    ('Développeur Junior', 'Développement sous supervision', 2000.00, GETDATE(), GETDATE()),
    ('Chargé RH', 'Gestion administrative RH', 2500.00, GETDATE(), GETDATE()),
    ('Commercial Senior', 'Vente et relation client', 3000.00, GETDATE(), GETDATE()),
    ('Commercial Junior', 'Prospection et vente', 1800.00, GETDATE(), GETDATE()),
    ('Comptable', 'Gestion comptable', 2800.00, GETDATE(), GETDATE()),
    ('Responsable Marketing', 'Stratégie marketing', 4000.00, GETDATE(), GETDATE()),
    ('Community Manager', 'Gestion réseaux sociaux', 2200.00, GETDATE(), GETDATE()),
    ('Technicien Production', 'Support production', 2000.00, GETDATE(), GETDATE()),
    ('Assistant', 'Support administratif', 1500.00, GETDATE(), GETDATE());

-- =============================================
-- 3. Employés
-- =============================================
INSERT INTO Employees (Matricule, FirstName, LastName, DateOfBirth, Address, Phone, Email, DepartmentId, PositionId, HireDate, ContractType, Status, CreatedDate, ModifiedDate)
VALUES
-- Direction
('2020001', 'Ahmed', 'Ben Ali', '1975-05-15', 'Tunis, Tunisie', '+216 98 123 456', 'ahmed.benali@hrms.tn', 1, 1, '2020-01-01', 'CDI', 0, GETDATE(), GETDATE()),

-- RH
('2021001', 'Fatma', 'Trabelsi', '1985-08-20', 'Ariana, Tunisie', '+216 98 234 567', 'fatma.trabelsi@hrms.tn', 2, 2, '2021-02-01', 'CDI', 0, GETDATE(), GETDATE()),
('2022001', 'Leila', 'Hamdi', '1990-03-12', 'Ben Arous, Tunisie', '+216 98 345 678', 'leila.hamdi@hrms.tn', 2, 6, '2022-03-15', 'CDI', 0, GETDATE(), GETDATE()),

-- IT
('2021002', 'Mohamed', 'Khalil', '1988-11-05', 'La Marsa, Tunisie', '+216 98 456 789', 'mohamed.khalil@hrms.tn', 3, 3, '2021-04-01', 'CDI', 0, GETDATE(), GETDATE()),
('2022002', 'Sami', 'Bouaziz', '1992-07-18', 'Manouba, Tunisie', '+216 98 567 890', 'sami.bouaziz@hrms.tn', 3, 4, '2022-05-01', 'CDI', 0, GETDATE(), GETDATE()),
('2023001', 'Nour', 'Mansour', '1995-02-28', 'Tunis, Tunisie', '+216 98 678 901', 'nour.mansour@hrms.tn', 3, 5, '2023-06-01', 'CDI', 0, GETDATE(), GETDATE()),
('2024001', 'Youssef', 'Gharbi', '1996-09-10', 'Sfax, Tunisie', '+216 98 789 012', 'youssef.gharbi@hrms.tn', 3, 5, '2024-01-15', 'CDD', 0, GETDATE(), GETDATE()),

-- Commercial
('2021003', 'Karim', 'Mezni', '1987-04-22', 'Sousse, Tunisie', '+216 98 890 123', 'karim.mezni@hrms.tn', 4, 7, '2021-07-01', 'CDI', 0, GETDATE(), GETDATE()),
('2023002', 'Amira', 'Jlassi', '1993-12-08', 'Monastir, Tunisie', '+216 98 901 234', 'amira.jlassi@hrms.tn', 4, 8, '2023-08-01', 'CDI', 0, GETDATE(), GETDATE()),

-- Finance
('2020002', 'Hichem', 'Sassi', '1982-06-30', 'Tunis, Tunisie', '+216 98 012 345', 'hichem.sassi@hrms.tn', 5, 9, '2020-03-01', 'CDI', 0, GETDATE(), GETDATE()),

-- Marketing
('2022003', 'Salma', 'Karoui', '1989-01-14', 'La Goulette, Tunisie', '+216 98 123 456', 'salma.karoui@hrms.tn', 6, 10, '2022-09-01', 'CDI', 0, GETDATE(), GETDATE()),
('2023003', 'Marwen', 'Romdhani', '1994-10-25', 'Carthage, Tunisie', '+216 98 234 567', 'marwen.romdhani@hrms.tn', 6, 11, '2023-10-01', 'CDI', 0, GETDATE(), GETDATE()),

-- Production
('2021004', 'Hatem', 'Belhadj', '1986-08-17', 'Bizerte, Tunisie', '+216 98 345 678', 'hatem.belhadj@hrms.tn', 7, 12, '2021-11-01', 'CDI', 0, GETDATE(), GETDATE()),
('2024002', 'Rania', 'Khemiri', '1997-03-20', 'Nabeul, Tunisie', '+216 98 456 789', 'rania.khemiri@hrms.tn', 7, 13, '2024-02-01', 'Stage', 0, GETDATE(), GETDATE());

-- =============================================
-- 4. Mise à jour des Managers
-- =============================================
UPDATE Departments SET ManagerId = 1 WHERE DepartmentId = 1; -- DG
UPDATE Departments SET ManagerId = 2 WHERE DepartmentId = 2; -- RH
UPDATE Departments SET ManagerId = 4 WHERE DepartmentId = 3; -- IT
UPDATE Departments SET ManagerId = 8 WHERE DepartmentId = 4; -- Commercial
UPDATE Departments SET ManagerId = 10 WHERE DepartmentId = 5; -- Finance
UPDATE Departments SET ManagerId = 11 WHERE DepartmentId = 6; -- Marketing
UPDATE Departments SET ManagerId = 13 WHERE DepartmentId = 7; -- Production

-- =============================================
-- 5. Salaires (Historique initial)
-- =============================================
INSERT INTO Salaries (EmployeeId, BaseSalary, EffectiveDate, Justification, CreatedDate)
SELECT EmployeeId,
       p.BaseSalary,
       e.HireDate,
       'Salaire initial à l''embauche',
       GETDATE()
FROM Employees e
         INNER JOIN Positions p ON e.PositionId = p.PositionId;

-- Augmentations pour quelques employés
INSERT INTO Salaries (EmployeeId, BaseSalary, EffectiveDate, EndDate, Justification, CreatedDate)
VALUES
    (5, 3800.00, '2023-06-01', NULL, 'Augmentation mérite - Excellent travail', GETDATE()),
    (8, 3200.00, '2023-01-01', NULL, 'Augmentation annuelle', GETDATE());

-- Clôturer anciens salaires
UPDATE Salaries SET EndDate = '2023-05-31' WHERE EmployeeId = 5 AND BaseSalary = 3500.00;
UPDATE Salaries SET EndDate = '2022-12-31' WHERE EmployeeId = 8 AND BaseSalary = 3000.00;

-- =============================================
-- 6. Primes
-- =============================================
INSERT INTO Bonuses (EmployeeId, BonusType, Amount, Date, Description, CreatedDate)
VALUES
    (1, 'Performance', 2000.00, '2023-12-31', 'Prime de fin d''année 2023', GETDATE()),
    (2, 'Performance', 1500.00, '2023-12-31', 'Prime de fin d''année 2023', GETDATE()),
    (4, 'Performance', 1200.00, '2023-12-31', 'Prime de fin d''année 2023', GETDATE()),
    (5, 'Objectifs', 800.00, '2023-06-30', 'Atteinte objectifs semestriels', GETDATE()),
    (8, 'Performance', 1000.00, '2023-12-31', 'Prime de fin d''année 2023', GETDATE()),
    (11, 'Objectifs', 900.00, '2024-01-15', 'Campagne marketing réussie', GETDATE());

-- =============================================
-- 7. Avantages
-- =============================================
INSERT INTO Benefits (BenefitType, Description, Value, CreatedDate, ModifiedDate)
VALUES
    ('Assurance Maladie', 'Couverture santé complète', 150.00, GETDATE(), GETDATE()),
    ('Transport', 'Indemnité de transport mensuelle', 100.00, GETDATE(), GETDATE()),
    ('Repas', 'Tickets restaurant', 80.00, GETDATE(), GETDATE()),
    ('Télécommunications', 'Forfait téléphone et internet', 50.00, GETDATE(), GETDATE()),
    ('Formation', 'Budget formation annuel', 200.00, GETDATE(), GETDATE());

-- =============================================
-- 8. Attribution Avantages aux Employés
-- =============================================
-- Tous les cadres ont assurance + transport + repas
INSERT INTO EmployeeBenefits (EmployeeId, BenefitId, StartDate, CreatedDate)
SELECT e.EmployeeId, b.BenefitId, e.HireDate, GETDATE()
FROM Employees e
         CROSS JOIN Benefits b
WHERE e.PositionId IN (1, 2, 3, 7, 9, 10)
  AND b.BenefitId IN (1, 2, 3);

-- Équipe IT a également télécommunications
INSERT INTO EmployeeBenefits (EmployeeId, BenefitId, StartDate, CreatedDate)
SELECT e.EmployeeId, 4, e.HireDate, GETDATE()
FROM Employees e
WHERE e.DepartmentId = 3;

-- Tous les employés permanents ont formation
INSERT INTO EmployeeBenefits (EmployeeId, BenefitId, StartDate, CreatedDate)
SELECT e.EmployeeId, 5, e.HireDate, GETDATE()
FROM Employees e
WHERE e.ContractType = 'CDI';

-- =============================================
-- 9. Équipements
-- =============================================
INSERT INTO Equipments (EquipmentType, SerialNumber, Brand, Model, PurchaseDate, Status, CreatedDate, ModifiedDate)
VALUES
    ('Ordinateur', 'PC001', 'Dell', 'Latitude 5520', '2023-01-15', 1, GETDATE(), GETDATE()),
    ('Ordinateur', 'PC002', 'HP', 'EliteBook 840', '2023-01-15', 1, GETDATE(), GETDATE()),
    ('Ordinateur', 'PC003', 'Lenovo', 'ThinkPad X1', '2023-06-10', 1, GETDATE(), GETDATE()),
    ('Ordinateur', 'PC004', 'Dell', 'Latitude 5520', '2024-01-20', 1, GETDATE(), GETDATE()),
    ('Ordinateur', 'PC005', 'HP', 'ProBook 450', '2024-02-01', 0, GETDATE(), GETDATE()),
    ('Téléphone', 'TEL001', 'Samsung', 'Galaxy S21', '2023-02-01', 1, GETDATE(), GETDATE()),
    ('Téléphone', 'TEL002', 'iPhone', '13 Pro', '2023-02-01', 1, GETDATE(), GETDATE()),
    ('Téléphone', 'TEL003', 'Xiaomi', 'Mi 11', '2023-08-15', 1, GETDATE(), GETDATE()),
    ('Téléphone', 'TEL004', 'Samsung', 'Galaxy A52', '2024-01-10', 0, GETDATE(), GETDATE()),
    ('Badge', 'BDG001', 'HID', 'ProxCard II', '2023-01-01', 1, GETDATE(), GETDATE()),
    ('Badge', 'BDG002', 'HID', 'ProxCard II', '2023-01-01', 1, GETDATE(), GETDATE()),
    ('Badge', 'BDG003', 'HID', 'ProxCard II', '2023-01-01', 1, GETDATE(), GETDATE());

-- =============================================
-- 10. Affectation Équipements
-- =============================================
INSERT INTO EquipmentAssignments (EquipmentId, EmployeeId, AssignmentDate, Condition, Notes, CreatedDate)
VALUES
    (1, 4, '2023-01-20', 'Neuf', 'PC pour chef de projet IT', GETDATE()),
    (2, 5, '2023-01-20', 'Neuf', 'PC développeur senior', GETDATE()),
    (3, 6, '2023-06-15', 'Neuf', 'PC développeur junior', GETDATE()),
    (4, 7, '2024-01-22', 'Neuf', 'PC développeur junior', GETDATE()),
    (6, 1, '2023-02-05', 'Neuf', 'Téléphone DG', GETDATE()),
    (7, 2, '2023-02-05', 'Neuf', 'Téléphone DRH', GETDATE()),
    (8, 8, '2023-08-20', 'Neuf', 'Téléphone commercial', GETDATE()),
    (10, 1, '2023-01-02', 'Neuf', 'Badge accès DG', GETDATE()),
    (11, 4, '2023-01-20', 'Neuf', 'Badge accès IT', GETDATE()),
    (12, 8, '2023-08-01', 'Neuf', 'Badge accès commercial', GETDATE());

-- =============================================
-- 11. Promotions
-- =============================================
INSERT INTO Promotions (EmployeeId, OldPositionId, NewPositionId, OldSalary, NewSalary, PromotionDate, Justification, CreatedDate)
VALUES
    (5, 5, 4, 2000.00, 3500.00, '2022-06-01', 'Promotion Junior vers Senior suite à excellentes performances', GETDATE()),
    (6, 13, 5, 1500.00, 2000.00, '2023-12-01', 'Fin de stage avec embauche', GETDATE());

-- =============================================
-- 12. Offres d'emploi
-- =============================================
INSERT INTO JobOffers (Title, Description, DepartmentId, PositionId, PostDate, ExpiryDate, Status, CreatedDate, ModifiedDate)
VALUES
    ('Développeur Full Stack',
     'Nous recherchons un développeur Full Stack expérimenté pour rejoindre notre équipe IT. Compétences requises: ASP.NET MVC, Entity Framework, SQL Server, JavaScript, HTML/CSS.',
     3, 4, '2024-01-15', '2024-03-15', 0, GETDATE(), GETDATE()),

    ('Commercial B2B',
     'Poste de commercial pour développer notre portefeuille clients B2B. Expérience en vente de solutions requise.',
     4, 8, '2024-02-01', '2024-04-01', 0, GETDATE(), GETDATE()),

    ('Assistant(e) RH',
     'Assistant RH pour support administratif: gestion dossiers, contrats, planification entretiens.',
     2, 13, '2024-01-20', '2024-03-20', 0, GETDATE(), GETDATE());

-- =============================================
-- 13. Candidats
-- =============================================
INSERT INTO Candidates (FirstName, LastName, Email, Phone, JobOfferId, Status, ApplicationDate)
VALUES
    ('Amine', 'Zouari', 'amine.zouari@email.com', '+216 98 111 222', 1, 0, '2024-01-18'),
    ('Sarah', 'Bouslama', 'sarah.bouslama@email.com', '+216 98 222 333', 1, 1, '2024-01-20'),
    ('Mehdi', 'Cherif', 'mehdi.cherif@email.com', '+216 98 333 444', 1, 0, '2024-01-22'),
    ('Insaf', 'Mabrouk', 'insaf.mabrouk@email.com', '+216 98 444 555', 2, 1, '2024-02-03'),
    ('Oussama', 'Harbaoui', 'oussama.harbaoui@email.com', '+216 98 555 666', 2, 0, '2024-02-05'),
    ('Rim', 'Khelifi', 'rim.khelifi@email.com', '+216 98 666 777', 3, 2, '2024-01-25');

-- =============================================
-- 14. Entretiens
-- =============================================
INSERT INTO Interviews (CandidateId, InterviewDate, Location, InterviewerName, Notes, Result, CreatedDate)
VALUES
    (2, '2024-01-25 10:00', 'Bureau RH - Tunis', 'Fatma Trabelsi', 'Bon profil technique, à confirmer', 0, GETDATE()),
    (4, '2024-02-08 14:00', 'Bureau RH - Tunis', 'Karim Mezni', 'Excellent candidat, forte expérience', 1, GETDATE()),
    (6, '2024-01-28 11:00', 'Bureau RH - Tunis', 'Fatma Trabelsi', 'Profil accepté, à embaucher', 1, GETDATE());

-- =============================================
-- 15. Utilisateurs (Authentification)
-- =============================================
-- Mots de passe hashés SHA256:
-- admin123 = ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f
-- manager123 = d74ff0ee8da3b9806b18c877dbf29bbde50b5bd8e4dad7a3a725000feb82e8f1
-- employee123 = 8c6744c9d42ec2cb9e8885b54ff744d0e0192be89fd46e4e2b3e6f8e6d54e2e9

INSERT INTO Users (Username, PasswordHash, Email, EmployeeId, Role, IsActive, CreatedDate, ModifiedDate)
VALUES
    ('admin', 'ef92b778bafe771e89245b89ecbc08a44a4e166c06659911881f383d4473e94f', 'admin@hrms.tn', 2, 0, 1, GETDATE(), GETDATE()),
    ('manager.it', 'd74ff0ee8da3b9806b18c877dbf29bbde50b5bd8e4dad7a3a725000feb82e8f1', 'mohamed.khalil@hrms.tn', 4, 1, 1, GETDATE(), GETDATE()),
    ('employee1', '8c6744c9d42ec2cb9e8885b54ff744d0e0192be89fd46e4e2b3e6f8e6d54e2e9', 'sami.bouaziz@hrms.tn', 5, 2, 1, GETDATE(), GETDATE());

PRINT '================================================';
PRINT 'STATISTIQUES DE LA BASE DE DONNÉES';
PRINT '================================================';
PRINT '';

DECLARE @count INT;

-- Départements
SELECT @count = COUNT(*) FROM Departments;
PRINT 'Départements: ' + CAST(@count AS VARCHAR);

-- Postes
SELECT @count = COUNT(*) FROM Positions;
PRINT 'Postes: ' + CAST(@count AS VARCHAR);

-- Employés
SELECT @count = COUNT(*) FROM Employees;
PRINT 'Employés: ' + CAST(@count AS VARCHAR);

-- Employés actifs (Status = 0)
SELECT @count = COUNT(*) FROM Employees WHERE Status = 0;
PRINT 'Employés Actifs: ' + CAST(@count AS VARCHAR);

-- Salaires (historique)
SELECT @count = COUNT(*) FROM Salaries;
PRINT 'Salaires (historique): ' + CAST(@count AS VARCHAR);

-- Primes
SELECT @count = COUNT(*) FROM Bonuses;
PRINT 'Primes: ' + CAST(@count AS VARCHAR);

-- Avantages
SELECT @count = COUNT(*) FROM Benefits;
PRINT 'Avantages: ' + CAST(@count AS VARCHAR);

-- Avantages assignés
SELECT @count = COUNT(*) FROM EmployeeBenefits;
PRINT 'Avantages assignés: ' + CAST(@count AS VARCHAR);

-- Équipements
SELECT @count = COUNT(*) FROM Equipments;
PRINT 'Équipements: ' + CAST(@count AS VARCHAR);

-- Affectations équipements
SELECT @count = COUNT(*) FROM EquipmentAssignments;
PRINT 'Affectations: ' + CAST(@count AS VARCHAR);

-- Promotions
SELECT @count = COUNT(*) FROM Promotions;
PRINT 'Promotions: ' + CAST(@count AS VARCHAR);

-- Offres d'emploi
SELECT @count = COUNT(*) FROM JobOffers;
PRINT 'Offres d''emploi: ' + CAST(@count AS VARCHAR);

-- Candidats
SELECT @count = COUNT(*) FROM Candidates;
PRINT 'Candidats: ' + CAST(@count AS VARCHAR);

-- Entretiens
SELECT @count = COUNT(*) FROM Interviews;
PRINT 'Entretiens: ' + CAST(@count AS VARCHAR);

-- Utilisateurs
SELECT @count = COUNT(*) FROM Users;
PRINT 'Utilisateurs: ' + CAST(@count AS VARCHAR);

-- Masse salariale mensuelle (BaseSalary sans EndDate)
DECLARE @totalSalary DECIMAL(18,2);
SELECT @totalSalary = SUM(BaseSalary) FROM Salaries WHERE EndDate IS NULL;
PRINT 'Masse salariale mensuelle: ' + CAST(@totalSalary AS VARCHAR) + ' DT';

PRINT '';
PRINT '================================================';
PRINT 'INITIALISATION TERMINÉE AVEC SUCCÈS!';
PRINT '================================================';
PRINT '';
PRINT 'Comptes de test:';
PRINT '  Admin RH: admin / admin123';
PRINT '  Manager IT: manager.it / manager123';
PRINT '  Employé: employee1 / employee123';
PRINT '';
GO