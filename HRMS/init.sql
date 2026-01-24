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

-- Supprimer les données dans l'ordre enfants ? parents
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
    (1, 'Informatique', 'IT', 'Département informatique et développement', NULL, GETDATE(), GETDATE()),
    (2, 'Ressources Humaines', 'RH', 'Gestion du personnel et recrutement', NULL, GETDATE(), GETDATE()),
    (3, 'Finance', 'FIN', 'Comptabilité et gestion financière', NULL, GETDATE(), GETDATE()),
    (4, 'Marketing', 'MKT', 'Marketing digital et communication', NULL, GETDATE(), GETDATE()),
    (5, 'Commercial', 'COM', 'Ventes et relation client', NULL, GETDATE(), GETDATE()),
    (6, 'Production', 'PROD', 'Gestion de la production', NULL, GETDATE(), GETDATE()),
    (7, 'Logistique', 'LOG', 'Supply Chain et logistique', NULL, GETDATE(), GETDATE()),
    (8, 'Qualité', 'QUA', 'Contrôle qualité et normes', NULL, GETDATE(), GETDATE()),
    (9, 'R&D', 'RD', 'Recherche et développement', NULL, GETDATE(), GETDATE()),
    (10, 'Support Client', 'SUP', 'Service après-vente et support', NULL, GETDATE(), GETDATE());
SET IDENTITY_INSERT Departments OFF;
    
-- Positions
SET IDENTITY_INSERT Positions ON;
INSERT INTO Positions (PositionId, DepartmentId, Title, Description, BaseSalary, CreatedDate, ModifiedDate)
VALUES
    -- IT
    (1, 1, 'Développeur Junior', 'Développeur .NET débutant', 2500, GETDATE(), GETDATE()),
    (2, 1, 'Développeur Senior', 'Développeur .NET expérimenté', 4500, GETDATE(), GETDATE()),
    (3, 1, 'Architecte Logiciel', 'Architecture et conception', 6500, GETDATE(), GETDATE()),
    (4, 1, 'Chef de Projet IT', 'Gestion de projets informatiques', 5500, GETDATE(), GETDATE()),
    (5, 1, 'DevOps Engineer', 'Infrastructure et déploiement', 5000, GETDATE(), GETDATE()),

    -- RH & Admin
    (6, 2, 'Responsable RH', 'Direction des ressources humaines', 5000, GETDATE(), GETDATE()),
    (7, 2, 'Chargé de Recrutement', 'Recrutement et intégration', 3200, GETDATE(), GETDATE()),
    (8, 2, 'Assistant RH', 'Support administratif RH', 2200, GETDATE(), GETDATE()),

    -- Finance
    (9, 3, 'Directeur Financier', 'Direction financière', 7000, GETDATE(), GETDATE()),
    (10, 3, 'Comptable Senior', 'Comptabilité générale', 4000, GETDATE(), GETDATE()),
    (11, 3, 'Contrôleur de Gestion', 'Analyse et contrôle', 4500, GETDATE(), GETDATE()),

    -- Marketing & Commercial
    (12, 4, 'Responsable Marketing', 'Direction marketing', 5500, GETDATE(), GETDATE()),
    (13, 4, 'Digital Marketing Specialist', 'Marketing digital', 3800, GETDATE(), GETDATE()),
    (14, 5, 'Commercial Senior', 'Vente et négociation', 3500, GETDATE(), GETDATE()),
    (15, 5, 'Responsable Commercial', 'Direction commerciale', 6000, GETDATE(), GETDATE()),

    -- Autres
    (16, 6, 'Responsable Production', 'Gestion de production', 5200, GETDATE(), GETDATE()),
    (17, 7, 'Responsable Logistique', 'Supply chain management', 4800, GETDATE(), GETDATE()),
    (18, 8, 'Ingénieur Qualité', 'Assurance qualité', 4200, GETDATE(), GETDATE()),
    (19, 9, 'Chercheur R&D', 'Recherche et innovation', 5500, GETDATE(), GETDATE()),
    (20, 10, 'Support Technique', 'Assistance technique', 2800, GETDATE(), GETDATE());
SET IDENTITY_INSERT Positions OFF;

-- Employees
SET IDENTITY_INSERT Employees ON;
INSERT INTO Employees (EmployeeId, Matricule, FirstName, LastName, Gender, DateOfBirth, Address, Phone, Email, DepartmentId, PositionId, HireDate, ContractType, Status, PhotoPath, CreatedDate, ModifiedDate)
VALUES
    -- IT Department (10 employés)
    (1, '2020001', 'Mohamed', 'Ben Ali', 1,'1985-03-15', '12 Rue de la Liberté, Tunis', '20123456', 'mohamed.benali@hrms.tn', 1, 4, '2020-01-15', 0, 0, NULL, GETDATE(), GETDATE()),
    (2, '2020002', 'Amira', 'Trabelsi',2, '1990-07-22', '45 Avenue Bourguiba, Tunis', '20244567', 'amira.trabelsi@hrms.tn', 1, 3, '2020-03-01', 0, 0, NULL, GETDATE(), GETDATE()),
    (3, '2021003', 'Karim', 'Jebali', 1,'1992-11-08', '78 Rue Ibn Khaldoun, Sousse', '21345678', 'karim.jebali@hrms.tn', 1, 2, '2021-06-10', 0, 0, NULL, GETDATE(), GETDATE()),
    (4, '2021004', 'Sarra', 'Hamdi', 2,'1995-02-14', '23 Avenue de France, Tunis', '21456789', 'sarra.hamdi@hrms.tn', 1, 2, '2021-08-20', 0, 0, NULL, GETDATE(), GETDATE()),
    (5, '2022005', 'Youssef', 'Khelifi', 1,'1996-05-30', '56 Rue de Marseille, Sfax', '22567890', 'youssef.khelifi@hrms.tn', 1, 1, '2022-01-10', 0, 0, NULL, GETDATE(), GETDATE()),
    (6, '2022006', 'Nadia', 'Gharbi', 2,'1993-09-18', '89 Avenue Habib Thameur, Tunis', '22678901', 'nadia.gharbi@hrms.tn', 1, 1, '2022-04-05', 1, 0, NULL, GETDATE(), GETDATE()),
    (7, '2024007', 'Mehdi', 'Sassi', 1,'1994-12-25', '34 Rue Charles de Gaulle, Bizerte', '23789012', 'mehdi.sassi@hrms.tn', 1, 1, '2024-02-15', 0, 0, NULL, GETDATE(), GETDATE()),
    (8, '2024008', 'Lina', 'Bouaziz', 2,'1997-04-07', '67 Avenue de Carthage, Tunis', '23890123', 'lina.bouaziz@hrms.tn', 1, 5, '2024-05-20', 0, 0, NULL, GETDATE(), GETDATE()),
    (9, '2025009', 'Ahmed', 'Mejri', 1,'1998-08-12', '12 Rue de Rome, Monastir', '24901234', 'ahmed.mejri@hrms.tn', 1, 1, '2025-01-08', 1, 0, NULL, GETDATE(), GETDATE()),
    (10, '2025010', 'Rim', 'Souissi', 2,'1999-01-20', '45 Avenue Mohamed V, Tunis', '24012345', 'rim.souissi@hrms.tn', 1, 1, '2025-03-12', 0, 0, NULL, GETDATE(), GETDATE()),

    -- RH Department (5 employés)
    (11, '2019011', 'Salma', 'Abidi', 2,'1982-06-10', '78 Rue de la République, Tunis', '19123456', 'salma.abidi@hrms.tn', 2, 6, '2019-05-01', 0, 0, NULL, GETDATE(), GETDATE()),
    (12, '2020012', 'Hichem', 'Najar',1, '1988-10-25', '23 Avenue Habib Bourguiba, Sousse', '20244567', 'hichem.najar@hrms.tn', 2, 7, '2020-07-15', 0, 0, NULL, GETDATE(), GETDATE()),
    (13, '2021013', 'Inès', 'Belhaj', 2,'1991-03-18', '56 Rue de Paris, Tunis', '21345678', 'ines.belhaj@hrms.tn', 2, 7, '2021-09-10', 0, 0, NULL, GETDATE(), GETDATE()),
    (14, '2022014', 'Fares', 'Khiari',1, '1993-07-05', '89 Avenue de la Liberté, Sfax', '22456789', 'fares.khiari@hrms.tn', 2, 8, '2022-03-20', 1, 0, NULL, GETDATE(), GETDATE()),
    (15, '2024015', 'Marwa', 'Rekik', 2,'1995-11-30', '34 Rue Ibn Sina, Tunis', '23567890', 'marwa.rekik@hrms.tn', 2, 8, '2024-06-01', 0, 0, NULL, GETDATE(), GETDATE()),

    -- Finance Department (6 employés)
    (16, '2018016', 'Tarek', 'Chaabane',1, '1980-04-12', '67 Avenue de France, Tunis', '18123456', 'tarek.chaabane@hrms.tn', 3, 9, '2018-02-01', 0, 0, NULL, GETDATE(), GETDATE()),
    (17, '2019017', 'Samia', 'Boussetta', 2,'1986-08-28', '12 Rue de Marseille, La Marsa', '19234567', 'samia.boussetta@hrms.tn', 3, 10, '2019-09-15', 0, 0, NULL, GETDATE(), GETDATE()),
    (18, '2020018', 'Ramzi', 'Zouari', 1,'1989-12-03', '45 Avenue Habib Thameur, Tunis', '20345678', 'ramzi.zouari@hrms.tn', 3, 10, '2020-11-20', 0, 0, NULL, GETDATE(), GETDATE()),
    (19, '2021019', 'Houda', 'Mansour', 2,'1992-02-16', '78 Rue Charles Nicolle, Sousse', '21456789', 'houda.mansour@hrms.tn', 3, 11, '2021-04-10', 0, 0, NULL, GETDATE(), GETDATE()),
    (20, '2024020', 'Bilel', 'Gargouri', 1,'1994-06-22', '23 Avenue de Carthage, Tunis', '23567890', 'bilel.gargouri@hrms.tn', 3, 10, '2024-01-15', 1, 0, NULL, GETDATE(), GETDATE()),
    (21, '2025021', 'Wafa', 'Mrad', 2,'1996-10-08', '56 Rue de la Kasbah, Tunis', '24678901', 'wafa.mrad@hrms.tn', 3, 11, '2025-02-01', 0, 0, NULL, GETDATE(), GETDATE()),

    -- Marketing Department (6 employés)
    (22, '2019022', 'Nizar', 'Hamza', 1,'1984-05-15', '89 Avenue Mohamed V, Tunis', '19345678', 'nizar.hamza@hrms.tn', 4, 12, '2019-03-10', 0, 0, NULL, GETDATE(), GETDATE()),
    (23, '2020023', 'Asma', 'Chahed', 2,'1990-09-20', '34 Rue de Rome, Sfax', '20456789', 'asma.chahed@hrms.tn', 4, 13, '2020-05-25', 0, 0, NULL, GETDATE(), GETDATE()),
    (24, '2021024', 'Slim', 'Oueslati', 1,'1993-01-12', '67 Avenue de la République, Tunis', '21567890', 'slim.oueslati@hrms.tn', 4, 13, '2021-07-18', 0, 0, NULL, GETDATE(), GETDATE()),
    (25, '2022025', 'Leila', 'Dridi', 2,'1995-04-28', '12 Rue Ibn Khaldoun, Bizerte', '22678901', 'leila.dridi@hrms.tn', 4, 13, '2022-09-05', 1, 0, NULL, GETDATE(), GETDATE()),
    (26, '2024026', 'Oussama', 'Fatnassi', 1,'1997-08-14', '45 Avenue Habib Bourguiba, Tunis', '23789012', 'oussama.fatnassi@hrms.tn', 4, 13, '2024-11-20', 0, 0, NULL, GETDATE(), GETDATE()),
    (27, '2025027', 'Sawsen', 'Arbi', 2,'1998-12-01', '78 Rue de la Liberté, Monastir', '24890123', 'sawsen.arbi@hrms.tn', 4, 13, '2025-01-15', 0, 0, NULL, GETDATE(), GETDATE()),

    -- Commercial Department (8 employés)
    (28, '2018028', 'Zied', 'Tlili',1, '1983-03-22', '23 Avenue de France, Tunis', '18456789', 'zied.tlili@hrms.tn', 5, 15, '2018-06-01', 0, 0, NULL, GETDATE(), GETDATE()),
    (29, '2019029', 'Hajer', 'Mkacher', 2,'1987-07-18', '56 Rue de Marseille, Sousse', '19567890', 'hajer.mkacher@hrms.tn', 5, 14, '2019-10-15', 0, 0, NULL, GETDATE(), GETDATE()),
    (30, '2020030', 'Chokri', 'Amri',1, '1990-11-05', '89 Avenue Habib Thameur, Tunis', '20678901', 'chokri.amri@hrms.tn', 5, 14, '2020-12-20', 0, 0, NULL, GETDATE(), GETDATE()),
    (31, '2021031', 'Sonia', 'Yahia', 2,'1992-02-28', '34 Rue Charles de Gaulle, Sfax', '21789012', 'sonia.yahia@hrms.tn', 5, 14, '2021-05-10', 0, 0, NULL, GETDATE(), GETDATE()),
    (32, '2022032', 'Maher', 'Kammoun',1, '1994-06-14', '67 Avenue de Carthage, Tunis', '22890123', 'maher.kammoun@hrms.tn', 5, 14, '2022-08-25', 1, 0, NULL, GETDATE(), GETDATE()),
    (33, '2024033', 'Nesrine', 'Jendoubi', 2,'1996-10-30', '12 Rue de Rome, Bizerte', '23901234', 'nesrine.jendoubi@hrms.tn', 5, 14, '2024-03-15', 0, 0, NULL, GETDATE(), GETDATE()),
    (34, '2025034', 'Faouzi', 'Dhouib', 1,'1997-01-25', '45 Avenue Mohamed V, Tunis', '24012345', 'faouzi.dhouib@hrms.tn', 5, 14, '2025-06-01', 0, 0, NULL, GETDATE(), GETDATE()),
    (35, '2025035', 'Radhia', 'Chaari', 2,'1998-05-12', '78 Rue de la République, Monastir', '24123456', 'radhia.chaari@hrms.tn', 5, 14, '2025-09-10', 0, 0, NULL, GETDATE(), GETDATE()),

    -- Production Department (5 employés)
    (36, '2019036', 'Kamel', 'Ferchichi', 1,'1985-09-08', '23 Avenue Habib Bourguiba, Tunis', '19678901', 'kamel.ferchichi@hrms.tn', 6, 16, '2019-04-01', 0, 0, NULL, GETDATE(), GETDATE()),
    (37, '2020037', 'Monia', 'Kharrat', 2,'1988-12-15', '56 Rue de Paris, Sousse', '20789012', 'monia.kharrat@hrms.tn', 6, 16, '2020-08-20', 0, 0, NULL, GETDATE(), GETDATE()),
    (38, '2021038', 'Hassen', 'Sghaier',1, '1991-04-22', '89 Avenue de la Liberté, Sfax', '21890123', 'hassen.sghaier@hrms.tn', 6, 16, '2021-11-10', 0, 0, NULL, GETDATE(), GETDATE()),
    (39, '2024039', 'Jalila', 'Bouzid', 2,'1993-08-07', '34 Rue Ibn Sina, Tunis', '23012345', 'jalila.bouzid@hrms.tn', 6, 16, '2024-02-28', 1, 0, NULL, GETDATE(), GETDATE()),
    (40, '2025040', 'Ridha', 'Aloui', 1,'1995-11-19', '67 Avenue de France, Bizerte', '24234567', 'ridha.aloui@hrms.tn', 6, 16, '2025-05-15', 0, 0, NULL, GETDATE(), GETDATE()),

    -- Autres départements (10 employés)
    (41, '2020041', 'Moez', 'Belaid', 1,'1986-02-10', '12 Rue de Marseille, Tunis', '20890123', 'moez.belaid@hrms.tn', 7, 17, '2020-02-15', 0, 0, NULL, GETDATE(), GETDATE()),
    (42, '2021042', 'Amel', 'Chebbi', 2,'1989-06-25', '45 Avenue Habib Thameur, La Marsa', '21901234', 'amel.chebbi@hrms.tn', 7, 17, '2021-10-05', 0, 0, NULL, GETDATE(), GETDATE()),
    (43, '2019043', 'Riadh', 'Hammami', 1,'1984-10-12', '78 Rue Charles Nicolle, Tunis', '19789012', 'riadh.hammami@hrms.tn', 8, 18, '2019-07-20', 0, 0, NULL, GETDATE(), GETDATE()),
    (44, '2022044', 'Imen', 'Mathlouthi', 2,'1992-03-28', '23 Avenue de Carthage, Sousse', '22123456', 'imen.mathlouthi@hrms.tn', 8, 18, '2022-12-10', 0, 0, NULL, GETDATE(), GETDATE()),
    (45, '2018045', 'Khaled', 'Bouslama',1, '1981-07-15', '56 Rue de la Kasbah, Tunis', '18567890', 'khaled.bouslama@hrms.tn', 9, 19, '2018-09-01', 0, 0, NULL, GETDATE(), GETDATE()),
    (46, '2021046', 'Sihem', 'Daoud', 2,'1990-11-22', '89 Avenue Mohamed V, Sfax', '21012345', 'sihem.daoud@hrms.tn', 9, 19, '2021-03-25', 0, 0, NULL, GETDATE(), GETDATE()),
    (47, '2024047', 'Nabil', 'Ghariani',1, '1994-05-08', '34 Rue de Rome, Tunis', '23123456', 'nabil.ghariani@hrms.tn', 10, 20, '2024-08-15', 0, 0, NULL, GETDATE(), GETDATE()),
    (48, '2025048', 'Olfa', 'Mezni', 2,'1996-09-14', '67 Avenue de la République, Bizerte', '24345678', 'olfa.mezni@hrms.tn', 10, 20, '2025-04-20', 0, 0, NULL, GETDATE(), GETDATE()),
    (49, '2025049', 'Wassim', 'Agrebi',1, '1997-01-30', '12 Rue Ibn Khaldoun, Monastir', '24456789', 'wassim.agrebi@hrms.tn', 10, 20, '2025-07-10', 0, 0, NULL, GETDATE(), GETDATE()),
    (50, '2025050', 'Ahlem', 'Triki', 2,'1998-04-18', '45 Avenue Habib Bourguiba, Tunis', '24567890', 'ahlem.triki@hrms.tn', 10, 20, '2025-10-05', 0, 0, NULL, GETDATE(), GETDATE());
SET IDENTITY_INSERT Employees OFF;

-- =====================================================
-- MISE À JOUR DES MANAGERS DE DÉPARTEMENTS
-- =====================================================

UPDATE Departments SET ManagerId = 1 WHERE DepartmentId = 1;  -- IT
UPDATE Departments SET ManagerId = 11 WHERE DepartmentId = 2; -- RH
UPDATE Departments SET ManagerId = 16 WHERE DepartmentId = 3; -- Finance
UPDATE Departments SET ManagerId = 22 WHERE DepartmentId = 4; -- Marketing
UPDATE Departments SET ManagerId = 28 WHERE DepartmentId = 5; -- Commercial
UPDATE Departments SET ManagerId = 36 WHERE DepartmentId = 6; -- Production
UPDATE Departments SET ManagerId = 41 WHERE DepartmentId = 7; -- Logistique
UPDATE Departments SET ManagerId = 43 WHERE DepartmentId = 8; -- Qualité
UPDATE Departments SET ManagerId = 45 WHERE DepartmentId = 9; -- R&D
UPDATE Departments SET ManagerId = 47 WHERE DepartmentId = 10; -- Support

-- =====================================================
-- INSERTION DES UTILISATEURS (15 utilisateurs)
-- =====================================================

SET IDENTITY_INSERT Users ON;
INSERT INTO Users (UserId, Username, PasswordHash, Email, EmployeeId, Role, IsActive, CreatedDate, ModifiedDate)
VALUES
    -- AdminRH
    (1, 'admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'admin@hrms.tn', 11, 0, 1, GETDATE(), GETDATE()),

    -- Managers
    (3, 'manager.it', '866485796cfa8d7c0cf7111640205b83076433547577511d81f8030ae99ecea5', 'mohamed.benali@hrms.tn', 1, 1, 1, GETDATE(), GETDATE()),
    (4, 'tarek.chaabane', '866485796cfa8d7c0cf7111640205b83076433547577511d81f8030ae99ecea5', 'tarek.chaabane@hrms.tn', 16, 1, 1, GETDATE(), GETDATE()),
    (5, 'nizar.hamza', '866485796cfa8d7c0cf7111640205b83076433547577511d81f8030ae99ecea5', 'nizar.hamza@hrms.tn', 22, 1, 1, GETDATE(), GETDATE()),
    (6, 'zied.tlili', '866485796cfa8d7c0cf7111640205b83076433547577511d81f8030ae99ecea5', 'zied.tlili@hrms.tn', 28, 1, 1, GETDATE(), GETDATE()),

    -- Employees
    (7, 'employee1', '5b2f8e27e2e5b4081c03ce70b288c87bd1263140cbd1bd9ae078123509b7caff', 'amira.trabelsi@hrms.tn', 2, 2, 1, GETDATE(), GETDATE()),
    (8, 'karim.jebali', '5b2f8e27e2e5b4081c03ce70b288c87bd1263140cbd1bd9ae078123509b7caff', 'karim.jebali@hrms.tn', 3, 2, 1, GETDATE(), GETDATE()),
    (9, 'samia.boussetta', '5b2f8e27e2e5b4081c03ce70b288c87bd1263140cbd1bd9ae078123509b7caff', 'samia.boussetta@hrms.tn', 17, 2, 1, GETDATE(), GETDATE()),
    (10, 'asma.chahed', '5b2f8e27e2e5b4081c03ce70b288c87bd1263140cbd1bd9ae078123509b7caff', 'asma.chahed@hrms.tn', 23, 2, 1, GETDATE(), GETDATE()),
    (11, 'hajer.mkacher', '5b2f8e27e2e5b4081c03ce70b288c87bd1263140cbd1bd9ae078123509b7caff', 'hajer.mkacher@hrms.tn', 29, 2, 1, GETDATE(), GETDATE()),
    (12, 'hichem.najar', '5b2f8e27e2e5b4081c03ce70b288c87bd1263140cbd1bd9ae078123509b7caff', 'hichem.najar@hrms.tn', 12, 2, 1, GETDATE(), GETDATE()),
    (13, 'moez.belaid', '5b2f8e27e2e5b4081c03ce70b288c87bd1263140cbd1bd9ae078123509b7caff', 'moez.belaid@hrms.tn', 41, 2, 1, GETDATE(), GETDATE()),
    (14, 'khaled.bouslama', '5b2f8e27e2e5b4081c03ce70b288c87bd1263140cbd1bd9ae078123509b7caff', 'khaled.bouslama@hrms.tn', 45, 2, 1, GETDATE(), GETDATE()),
    (15, 'riadh.hammami', '5b2f8e27e2e5b4081c03ce70b288c87bd1263140cbd1bd9ae078123509b7caff', 'riadh.hammami@hrms.tn', 43, 2, 1, GETDATE(), GETDATE());
SET IDENTITY_INSERT Users OFF;

-- Benefits
SET IDENTITY_INSERT Benefits ON;
INSERT INTO Benefits (BenefitId, BenefitType, Description, Value, CreatedDate, ModifiedDate)
VALUES
    (1, 'Assurance Santé', 'Assurance médicale complète pour l''employé et sa famille', 250, GETDATE(), GETDATE()),
    (2, 'Ticket Restaurant', 'Ticket restaurant mensuel', 120, GETDATE(), GETDATE()),
    (3, 'Transport', 'Indemnité de transport', 180, GETDATE(), GETDATE()),
    (4, 'Téléphone', 'Forfait mobile professionnel', 60, GETDATE(), GETDATE()),
    (5, 'Formation', 'Budget formation annuel', 200, GETDATE(), GETDATE()),
    (6, 'Parking', 'Place de parking', 80, GETDATE(), GETDATE()),
    (7, 'Gym', 'Abonnement salle de sport', 100, GETDATE(), GETDATE()),
    (8, 'Congés supplémentaires', 'Jours de congés additionnels', 150, GETDATE(), GETDATE());
SET IDENTITY_INSERT Benefits OFF;

-- EmployeeBenefits
SET IDENTITY_INSERT EmployeeBenefits ON;
INSERT INTO EmployeeBenefits (EmployeeBenefitId, EmployeeId, BenefitId, StartDate, EndDate, CreatedDate)
VALUES
    -- Tous les employés CDI ont l'assurance santé
    (1, 1, 1, '2020-01-15', NULL, GETDATE()), (2, 2, 1, '2020-03-01', NULL, GETDATE()),
    (3, 3, 1, '2021-06-10', NULL, GETDATE()), (4, 4, 1, '2021-08-20', NULL, GETDATE()),
    (5, 5, 1, '2022-01-10', NULL, GETDATE()), (7, 7, 1, '2024-02-15', NULL, GETDATE()),
    (8, 8, 1, '2024-05-20', NULL, GETDATE()), (10, 10, 1, '2025-03-12', NULL, GETDATE()),
    (11, 11, 1, '2019-05-01', NULL, GETDATE()), (12, 12, 1, '2020-07-15', NULL, GETDATE()),
    (13, 13, 1, '2021-09-10', NULL, GETDATE()), (15, 15, 1, '2024-06-01', NULL, GETDATE()),
    (16, 16, 1, '2018-02-01', NULL, GETDATE()), (17, 17, 1, '2019-09-15', NULL, GETDATE()),
    (18, 18, 1, '2020-11-20', NULL, GETDATE()), (19, 19, 1, '2021-04-10', NULL, GETDATE()),
    (21, 21, 1, '2025-02-01', NULL, GETDATE()), (22, 22, 1, '2019-03-10', NULL, GETDATE()),
    (23, 23, 1, '2020-05-25', NULL, GETDATE()), (24, 24, 1, '2021-07-18', NULL, GETDATE()),
    (26, 26, 1, '2024-11-20', NULL, GETDATE()), (27, 27, 1, '2025-01-15', NULL, GETDATE()),

    -- Tickets restaurant pour les employés IT et Finance
    (28, 1, 2, '2020-01-15', NULL, GETDATE()), (29, 2, 2, '2020-03-01', NULL, GETDATE()),
    (30, 3, 2, '2021-06-10', NULL, GETDATE()), (31, 4, 2, '2021-08-20', NULL, GETDATE()),
    (32, 5, 2, '2022-01-10', NULL, GETDATE()), (33, 7, 2, '2024-02-15', NULL, GETDATE()),
    (34, 16, 2, '2018-02-01', NULL, GETDATE()), (35, 17, 2, '2019-09-15', NULL, GETDATE()),
    (36, 18, 2, '2020-11-20', NULL, GETDATE()), (37, 19, 2, '2021-04-10', NULL, GETDATE()),

    -- Transport pour certains employés
    (38, 5, 3, '2022-01-15', NULL, GETDATE()), (39, 7, 3, '2024-03-01', NULL, GETDATE()),
    (40, 10, 3, '2025-04-01', NULL, GETDATE()), (41, 13, 3, '2021-10-01', NULL, GETDATE()),
    (42, 23, 3, '2020-06-01', NULL, GETDATE()), (43, 26, 3, '2024-12-01', NULL, GETDATE()),
    (44, 29, 3, '2019-11-01', NULL, GETDATE()), (45, 33, 3, '2024-04-01', NULL, GETDATE()),

    -- Téléphone professionnel pour les managers et commerciaux
    (46, 1, 4, '2020-01-15', NULL, GETDATE()), (47, 11, 4, '2019-05-01', NULL, GETDATE()),
    (48, 16, 4, '2018-02-01', NULL, GETDATE()), (49, 22, 4, '2019-03-10', NULL, GETDATE()),
    (50, 28, 4, '2018-06-01', NULL, GETDATE()), (51, 29, 4, '2019-10-15', NULL, GETDATE()),
    (52, 30, 4, '2020-12-20', NULL, GETDATE()), (53, 31, 4, '2021-05-10', NULL, GETDATE()),
    (54, 33, 4, '2024-03-15', NULL, GETDATE()), (55, 34, 4, '2025-06-01', NULL, GETDATE()),

    -- Formation pour certains profils
    (56, 2, 5, '2020-03-01', NULL, GETDATE()), (57, 3, 5, '2021-06-10', NULL, GETDATE()),
    (58, 12, 5, '2020-07-15', NULL, GETDATE()), (59, 19, 5, '2021-04-10', NULL, GETDATE()),
    (60, 23, 5, '2020-05-25', NULL, GETDATE()), (61, 45, 5, '2018-09-01', NULL, GETDATE()),

    -- Parking pour les managers
    (62, 1, 6, '2020-01-15', NULL, GETDATE()), (63, 11, 6, '2019-05-01', NULL, GETDATE()),
    (64, 16, 6, '2018-02-01', NULL, GETDATE()), (65, 22, 6, '2019-03-10', NULL, GETDATE()),
    (66, 28, 6, '2018-06-01', NULL, GETDATE()), (67, 36, 6, '2019-04-01', NULL, GETDATE()),
    (68, 41, 6, '2020-02-15', NULL, GETDATE()), (69, 43, 6, '2019-07-20', NULL, GETDATE()),
    (70, 45, 6, '2018-09-01', NULL, GETDATE()), (71, 47, 6, '2024-08-15', NULL, GETDATE()),

    -- Gym pour certains employés
    (72, 2, 7, '2020-03-01', NULL, GETDATE()), (73, 8, 7, '2024-05-20', NULL, GETDATE()),
    (74, 12, 7, '2020-07-15', NULL, GETDATE()), (75, 23, 7, '2020-05-25', NULL, GETDATE()),
    (76, 29, 7, '2019-10-15', NULL, GETDATE()),

    -- Congés supplémentaires pour les anciens
    (77, 1, 8, '2024-01-01', NULL, GETDATE()), (78, 11, 8, '2022-01-01', NULL, GETDATE()),
    (79, 16, 8, '2021-01-01', NULL, GETDATE()), (80, 22, 8, '2022-01-01', NULL, GETDATE()),
    (81, 28, 8, '2021-01-01', NULL, GETDATE()), (82, 45, 8, '2021-01-01', NULL, GETDATE());
SET IDENTITY_INSERT EmployeeBenefits OFF;

-- Equipments
SET IDENTITY_INSERT Equipments ON;
INSERT INTO Equipments (EquipmentId, EquipmentType, SerialNumber, Brand, Model, PurchaseDate, Status, CreatedDate, ModifiedDate)
VALUES
    -- Ordinateurs portables (15)
    (1, 'Ordinateur Portable', 'SN-LAP-001', 'Dell', 'Latitude 5420', '2024-01-15', 1, GETDATE(), GETDATE()),
    (2, 'Ordinateur Portable', 'SN-LAP-002', 'HP', 'EliteBook 840', '2024-02-20', 1, GETDATE(), GETDATE()),
    (3, 'Ordinateur Portable', 'SN-LAP-003', 'Lenovo', 'ThinkPad X1', '2024-03-10', 1, GETDATE(), GETDATE()),
    (4, 'Ordinateur Portable', 'SN-LAP-004', 'Dell', 'Latitude 7420', '2024-04-05', 1, GETDATE(), GETDATE()),
    (5, 'Ordinateur Portable', 'SN-LAP-005', 'HP', 'ProBook 450', '2024-05-12', 1, GETDATE(), GETDATE()),
    (6, 'Ordinateur Portable', 'SN-LAP-006', 'Asus', 'ZenBook 14', '2024-06-18', 1, GETDATE(), GETDATE()),
    (7, 'Ordinateur Portable', 'SN-LAP-007', 'Dell', 'Precision 5560', '2024-07-22', 1, GETDATE(), GETDATE()),
    (8, 'Ordinateur Portable', 'SN-LAP-008', 'HP', 'EliteBook 850', '2024-08-30', 1, GETDATE(), GETDATE()),
    (9, 'Ordinateur Portable', 'SN-LAP-009', 'Lenovo', 'ThinkPad T14', '2024-09-14', 1, GETDATE(), GETDATE()),
    (10, 'Ordinateur Portable', 'SN-LAP-010', 'Dell', 'Latitude 5520', '2024-10-25', 0, GETDATE(), GETDATE()),
    (11, 'Ordinateur Portable', 'SN-LAP-011', 'HP', 'ProBook 440', '2025-01-08', 0, GETDATE(), GETDATE()),
    (12, 'Ordinateur Portable', 'SN-LAP-012', 'Lenovo', 'IdeaPad 5', '2025-02-15', 0, GETDATE(), GETDATE()),
    (13, 'Ordinateur Portable', 'SN-LAP-013', 'Asus', 'VivoBook 15', '2025-03-20', 0, GETDATE(), GETDATE()),
    (14, 'Ordinateur Portable', 'SN-LAP-014', 'Dell', 'Inspiron 15', '2025-04-10', 0, GETDATE(), GETDATE()),
    (15, 'Ordinateur Portable', 'SN-LAP-015', 'HP', 'Pavilion 15', '2025-05-05', 0, GETDATE(), GETDATE()),

    -- Téléphones (8)
    (16, 'Téléphone', 'SN-TEL-001', 'Samsung', 'Galaxy S23', '2024-02-10', 1, GETDATE(), GETDATE()),
    (17, 'Téléphone', 'SN-TEL-002', 'iPhone', '14 Pro', '2024-03-15', 1, GETDATE(), GETDATE()),
    (18, 'Téléphone', 'SN-TEL-003', 'Samsung', 'Galaxy S22', '2024-04-20', 1, GETDATE(), GETDATE()),
    (19, 'Téléphone', 'SN-TEL-004', 'iPhone', '13', '2024-05-25', 1, GETDATE(), GETDATE()),
    (20, 'Téléphone', 'SN-TEL-005', 'Xiaomi', 'Mi 12', '2024-06-30', 1, GETDATE(), GETDATE()),
    (21, 'Téléphone', 'SN-TEL-006', 'Samsung', 'Galaxy A54', '2025-01-12', 0, GETDATE(), GETDATE()),
    (22, 'Téléphone', 'SN-TEL-007', 'iPhone', '15', '2025-02-18', 0, GETDATE(), GETDATE()),
    (23, 'Téléphone', 'SN-TEL-008', 'Xiaomi', 'Redmi Note 12', '2025-03-22', 0, GETDATE(), GETDATE()),

    -- Écrans (7)
    (24, 'Écran', 'SN-MON-001', 'Dell', 'UltraSharp 27"', '2024-01-20', 1, GETDATE(), GETDATE()),
    (25, 'Écran', 'SN-MON-002', 'LG', 'UltraWide 34"', '2024-03-25', 1, GETDATE(), GETDATE()),
    (26, 'Écran', 'SN-MON-003', 'Samsung', 'Curved 32"', '2024-05-10', 1, GETDATE(), GETDATE()),
    (27, 'Écran', 'SN-MON-004', 'BenQ', 'Professional 27"', '2024-07-15', 1, GETDATE(), GETDATE()),
    (28, 'Écran', 'SN-MON-005', 'HP', 'E24 G4', '2024-09-20', 0, GETDATE(), GETDATE()),
    (29, 'Écran', 'SN-MON-006', 'Dell', 'P2422H 24"', '2025-01-30', 0, GETDATE(), GETDATE()),
    (30, 'Écran', 'SN-MON-007', 'LG', '27UK850-W 27"', '2025-03-05', 0, GETDATE(), GETDATE());
SET IDENTITY_INSERT Equipments OFF;

-- =====================================================
-- AFFECTATIONS D'ÉQUIPEMENTS (25 affectations)
-- =====================================================

SET IDENTITY_INSERT EquipmentAssignments ON;
INSERT INTO EquipmentAssignments (AssignmentId, EquipmentId, EmployeeId, AssignmentDate, ReturnDate, Condition, Notes, CreatedDate)
VALUES
    -- Portables affectés
    (1, 1, 1, '2024-01-20', NULL, 'Neuf', 'Laptop manager IT', GETDATE()),
    (2, 2, 2, '2024-02-25', NULL, 'Neuf', 'Laptop architecte', GETDATE()),
    (3, 3, 3, '2024-03-15', NULL, 'Neuf', 'Laptop dev senior', GETDATE()),
    (4, 4, 11, '2024-04-10', NULL, 'Neuf', 'Laptop manager RH', GETDATE()),
    (5, 5, 16, '2024-05-18', NULL, 'Neuf', 'Laptop directeur financier', GETDATE()),
    (6, 6, 22, '2024-06-22', NULL, 'Neuf', 'Laptop manager marketing', GETDATE()),
    (7, 7, 28, '2024-07-28', NULL, 'Neuf', 'Laptop manager commercial', GETDATE()),
    (8, 8, 12, '2024-09-05', NULL, 'Bon', 'Laptop chargé recrutement', GETDATE()),
    (9, 9, 17, '2024-09-20', NULL, 'Bon', 'Laptop comptable', GETDATE()),

    -- Téléphones affectés
    (10, 16, 1, '2024-02-15', NULL, 'Neuf', 'Téléphone pro', GETDATE()),
    (11, 17, 11, '2024-03-20', NULL, 'Neuf', 'Téléphone pro', GETDATE()),
    (12, 18, 16, '2024-04-25', NULL, 'Neuf', 'Téléphone pro', GETDATE()),
    (13, 19, 22, '2024-05-30', NULL, 'Neuf', 'Téléphone pro', GETDATE()),
    (14, 20, 28, '2024-07-05', NULL, 'Neuf', 'Téléphone pro', GETDATE()),

    -- Écrans affectés
    (15, 24, 1, '2024-01-25', NULL, 'Neuf', 'Écran secondaire', GETDATE()),
    (16, 25, 2, '2024-03-30', NULL, 'Neuf', 'Écran large', GETDATE()),
    (17, 26, 3, '2024-05-15', NULL, 'Neuf', 'Écran courbe', GETDATE()),
    (18, 27, 16, '2024-07-20', NULL, 'Neuf', 'Écran professionnel', GETDATE()),

    -- Équipements retournés (historique)
    (19, 10, 5, '2024-11-01', '2025-04-15', 'Bon', 'Laptop temporaire - retourné', GETDATE()),
    (20, 11, 7, '2025-01-15', '2025-06-20', 'Acceptable', 'Laptop prêt - retourné', GETDATE()),
    (21, 28, 12, '2024-09-25', '2025-02-10', 'Bon', 'Écran temporaire - retourné', GETDATE()),
    (22, 29, 23, '2025-02-05', '2025-08-15', 'Bon', 'Écran prêt - retourné', GETDATE()),

    -- Nouvelles affectations récentes
    (23, 10, 10, '2025-04-20', NULL, 'Reconditionné', 'Laptop réaffecté', GETDATE()),
    (24, 11, 9, '2025-06-25', NULL, 'Reconditionné', 'Laptop réaffecté', GETDATE()),
    (25, 28, 19, '2025-02-15', NULL, 'Bon', 'Écran réaffecté', GETDATE());
SET IDENTITY_INSERT EquipmentAssignments OFF;

-- Salaries
SET IDENTITY_INSERT Salaries ON;
INSERT INTO Salaries (SalaryId, EmployeeId, BaseSalary, EffectiveDate, EndDate, Justification, CreatedDate)
VALUES
    -- Salaires actuels pour tous les employés
    (1, 1, 6500, '2024-01-01', NULL, 'Augmentation annuelle 2024', GETDATE()),
    (2, 2, 7500, '2024-01-01', NULL, 'Augmentation annuelle 2024', GETDATE()),
    (3, 3, 5200, '2024-07-01', NULL, 'Promotion senior developer', GETDATE()),
    (4, 4, 5000, '2025-01-01', NULL, 'Augmentation annuelle 2025', GETDATE()),
    (5, 5, 2800, '2025-01-01', NULL, 'Augmentation junior', GETDATE()),
    (6, 6, 2650, '2024-01-01', NULL, 'Salaire CDD', GETDATE()),
    (7, 7, 2900, '2025-01-01', NULL, 'Augmentation junior', GETDATE()),
    (8, 8, 5500, '2025-01-01', NULL, 'Salaire DevOps', GETDATE()),
    (9, 9, 2600, '2025-01-01', NULL, 'Salaire initial', GETDATE()),
    (10, 10, 2550, '2025-03-12', NULL, 'Salaire initial', GETDATE()),

    (11, 11, 6000, '2024-01-01', NULL, 'Augmentation manager RH', GETDATE()),
    (12, 12, 3800, '2024-01-01', NULL, 'Augmentation chargé recrutement', GETDATE()),
    (13, 13, 3600, '2025-01-01', NULL, 'Augmentation chargé recrutement', GETDATE()),
    (14, 14, 2400, '2024-01-01', NULL, 'Salaire assistant RH', GETDATE()),
    (15, 15, 2350, '2025-01-01', NULL, 'Salaire assistant RH', GETDATE()),

    (16, 16, 8000, '2024-01-01', NULL, 'Salaire directeur financier', GETDATE()),
    (17, 17, 4800, '2024-01-01', NULL, 'Augmentation comptable senior', GETDATE()),
    (18, 18, 4600, '2025-01-01', NULL, 'Augmentation comptable senior', GETDATE()),
    (19, 19, 5200, '2025-01-01', NULL, 'Salaire contrôleur gestion', GETDATE()),
    (20, 20, 4200, '2025-01-01', NULL, 'Salaire comptable CDD', GETDATE()),
    (21, 21, 4900, '2025-02-01', NULL, 'Salaire contrôleur gestion', GETDATE()),

    (22, 22, 6500, '2024-01-01', NULL, 'Augmentation manager marketing', GETDATE()),
    (23, 23, 4500, '2024-01-01', NULL, 'Salaire specialist digital', GETDATE()),
    (24, 24, 4300, '2025-01-01', NULL, 'Augmentation specialist', GETDATE()),
    (25, 25, 3900, '2024-01-01', NULL, 'Salaire specialist CDD', GETDATE()),
    (26, 26, 4100, '2025-01-01', NULL, 'Augmentation specialist', GETDATE()),
    (27, 27, 3950, '2025-01-15', NULL, 'Salaire initial specialist', GETDATE()),

    (28, 28, 7500, '2024-01-01', NULL, 'Salaire manager commercial', GETDATE()),
    (29, 29, 4200, '2024-01-01', NULL, 'Augmentation commercial', GETDATE()),
    (30, 30, 4000, '2025-01-01', NULL, 'Augmentation commercial', GETDATE()),
    (31, 31, 3900, '2025-01-01', NULL, 'Augmentation commercial', GETDATE()),
    (32, 32, 3600, '2024-01-01', NULL, 'Salaire commercial CDD', GETDATE()),
    (33, 33, 3800, '2025-01-01', NULL, 'Augmentation commercial', GETDATE()),
    (34, 34, 3650, '2025-06-01', NULL, 'Salaire initial commercial', GETDATE()),
    (35, 35, 3550, '2025-09-10', NULL, 'Salaire initial commercial', GETDATE()),

    (36, 36, 6200, '2024-01-01', NULL, 'Salaire responsable production', GETDATE()),
    (37, 37, 5800, '2025-01-01', NULL, 'Augmentation responsable', GETDATE()),
    (38, 38, 5500, '2025-01-01', NULL, 'Augmentation responsable', GETDATE()),
    (39, 39, 5300, '2024-02-28', '2025-01-31', 'Salaire CDD suspendu', GETDATE()),
    (40, 40, 5400, '2025-05-15', NULL, 'Salaire initial responsable', GETDATE()),

    (41, 41, 5600, '2024-01-01', NULL, 'Salaire responsable logistique', GETDATE()),
    (42, 42, 5400, '2025-01-01', NULL, 'Augmentation responsable', GETDATE()),
    (43, 43, 5000, '2024-01-01', NULL, 'Salaire ingénieur qualité', GETDATE()),
    (44, 44, 4700, '2025-01-01', NULL, 'Augmentation ingénieur', GETDATE()),
    (45, 45, 6800, '2024-01-01', NULL, 'Salaire chercheur R&D', GETDATE()),
    (46, 46, 6200, '2025-01-01', NULL, 'Augmentation chercheur', GETDATE()),
    (47, 47, 3200, '2025-01-01', NULL, 'Salaire support technique', GETDATE()),
    (48, 48, 2950, '2025-04-20', NULL, 'Salaire initial support CDD', GETDATE()),
    (49, 49, 3100, '2025-07-10', NULL, 'Salaire initial support', GETDATE()),
    (50, 50, 3050, '2025-10-05', NULL, 'Salaire initial support', GETDATE()),

    -- Historique salaires (augmentations passées)
    (51, 1, 5500, '2020-01-15', '2022-12-31', 'Salaire initial manager', GETDATE()),
    (52, 1, 6000, '2022-01-01', '2022-12-31', 'Augmentation 2022', GETDATE()),
    (53, 2, 6500, '2020-03-01', '2022-12-31', 'Salaire initial architecte', GETDATE()),
    (54, 3, 4500, '2021-06-10', '2024-06-30', 'Salaire initial senior', GETDATE()),
    (55, 16, 7000, '2018-02-01', '2022-12-31', 'Salaire initial directeur', GETDATE()),
    (56, 16, 7500, '2022-01-01', '2022-12-31', 'Augmentation 2022', GETDATE()),
    (57, 22, 5500, '2019-03-10', '2022-12-31', 'Salaire initial manager', GETDATE()),
    (58, 22, 6000, '2022-01-01', '2022-12-31', 'Augmentation 2022', GETDATE()),
    (59, 28, 6000, '2018-06-01', '2022-12-31', 'Salaire initial manager', GETDATE()),
    (60, 28, 7000, '2022-01-01', '2022-12-31', 'Augmentation 2022', GETDATE());
SET IDENTITY_INSERT Salaries OFF;

-- Bonuses
SET IDENTITY_INSERT Bonuses ON;
INSERT INTO Bonuses (BonusId, EmployeeId, BonusType, Amount, Date, Description, CreatedDate)
VALUES
    -- Primes de performance 2024
    (1, 1, 'Prime Performance', 1200, '2024-12-15', 'Excellents résultats département IT', GETDATE()),
    (2, 2, 'Prime Performance', 1500, '2024-12-15', 'Innovation architecture', GETDATE()),
    (3, 3, 'Prime Performance', 800, '2024-12-15', 'Bon travail senior dev', GETDATE()),
    (4, 11, 'Prime Performance', 1100, '2024-12-15', 'Gestion exemplaire RH', GETDATE()),
    (5, 16, 'Prime Performance', 1800, '2024-12-15', 'Gestion financière excellente', GETDATE()),
    (6, 22, 'Prime Performance', 1400, '2024-12-15', 'Stratégie marketing réussie', GETDATE()),
    (7, 28, 'Prime Performance', 2000, '2024-12-15', 'Objectifs commerciaux dépassés', GETDATE()),
    -- Primes projet 2024
    (8, 3, 'Prime Projet', 600, '2024-06-30', 'Livraison projet CRM', GETDATE()),
    (9, 4, 'Prime Projet', 550, '2024-06-30', 'Livraison projet CRM', GETDATE()),
    (10, 8, 'Prime Projet', 700, '2024-09-15', 'Migration infrastructure cloud', GETDATE()),
    (11, 23, 'Prime Projet', 500, '2024-08-20', 'Campagne digitale réussie', GETDATE()),
    (12, 24, 'Prime Projet', 480, '2024-08-20', 'Campagne digitale réussie', GETDATE()),

    -- Primes d'assiduité 2024
    (13, 5, 'Prime Assiduité', 200, '2024-06-30', 'Assiduité semestrielle', GETDATE()),
    (14, 7, 'Prime Assiduité', 200, '2024-06-30', 'Assiduité semestrielle', GETDATE()),
    (15, 12, 'Prime Assiduité', 250, '2024-06-30', 'Assiduité semestrielle', GETDATE()),
    (16, 17, 'Prime Assiduité', 300, '2024-06-30', 'Assiduité semestrielle', GETDATE()),
    (17, 29, 'Prime Assiduité', 280, '2024-06-30', 'Assiduité semestrielle', GETDATE()),

    -- Primes de performance 2025
    (18, 1, 'Prime Performance', 1300, '2025-06-30', 'Mi-année excellente', GETDATE()),
    (19, 2, 'Prime Performance', 1600, '2025-06-30', 'Innovation continue', GETDATE()),
    (20, 11, 'Prime Performance', 1200, '2025-06-30', 'Recrutements réussis', GETDATE()),
    (21, 16, 'Prime Performance', 2000, '2025-06-30', 'Optimisation financière', GETDATE()),
    (22, 22, 'Prime Performance', 1500, '2025-06-30', 'Croissance marketing', GETDATE()),
    (23, 28, 'Prime Performance', 2200, '2025-06-30', 'Chiffre d''affaires record', GETDATE()),

    -- Primes exceptionnelles
    (24, 2, 'Prime Exceptionnelle', 2500, '2024-11-10', 'Architecture nouveau système', GETDATE()),
    (25, 16, 'Prime Exceptionnelle', 3000, '2024-10-25', 'Réduction coûts 20%', GETDATE()),
    (26, 28, 'Prime Exceptionnelle', 3500, '2025-03-15', 'Nouveau contrat majeur', GETDATE()),
    (27, 45, 'Prime Exceptionnelle', 2000, '2024-09-30', 'Brevet innovation', GETDATE()),

    -- Primes de parrainage
    (28, 12, 'Prime Parrainage', 300, '2024-05-20', 'Parrainage candidat recruté', GETDATE()),
    (29, 23, 'Prime Parrainage', 300, '2024-07-15', 'Parrainage candidat recruté', GETDATE()),
    (30, 29, 'Prime Parrainage', 300, '2025-02-10', 'Parrainage candidat recruté', GETDATE()),

    -- Primes formation
    (31, 3, 'Prime Formation', 400, '2024-04-30', 'Certification Azure', GETDATE()),
    (32, 4, 'Prime Formation', 400, '2024-05-15', 'Certification AWS', GETDATE()),
    (33, 19, 'Prime Formation', 350, '2024-06-20', 'Certification CPA', GETDATE()),
    (34, 23, 'Prime Formation', 300, '2025-01-25', 'Certification Google Ads', GETDATE()),

    -- Primes diverses 2025
    (35, 8, 'Prime Projet', 800, '2025-04-15', 'Sécurisation infrastructure', GETDATE()),
    (36, 18, 'Prime Projet', 450, '2025-05-20', 'Audit financier complet', GETDATE()),
    (37, 30, 'Prime Performance', 500, '2025-08-10', 'Objectif trimestriel atteint', GETDATE()),
    (38, 33, 'Prime Performance', 480, '2025-08-10', 'Objectif trimestriel atteint', GETDATE()),
    (39, 42, 'Prime Performance', 600, '2025-07-15', 'Optimisation supply chain', GETDATE()),
    (40, 46, 'Prime Projet', 900, '2025-09-05', 'Développement nouveau produit', GETDATE());
SET IDENTITY_INSERT Bonuses OFF;

-- Documents
SET IDENTITY_INSERT Documents ON;
INSERT INTO Documents (DocumentId, EmployeeId, DocumentType, FileName, FilePath, UploadDate)
VALUES
    -- CVs
    (1, 1, 'CV', 'CV_Mohamed_BenAli.pdf', '/docs/cvs/mohamed_benali.pdf', '2020-01-10'),
    (2, 2, 'CV', 'CV_Amira_Trabelsi.pdf', '/docs/cvs/amira_trabelsi.pdf', '2020-02-25'),
    (3, 3, 'CV', 'CV_Karim_Jebali.pdf', '/docs/cvs/karim_jebali.pdf', '2021-06-05'),
    (4, 11, 'CV', 'CV_Salma_Abidi.pdf', '/docs/cvs/salma_abidi.pdf', '2019-04-25'),
    (5, 16, 'CV', 'CV_Tarek_Chaabane.pdf', '/docs/cvs/tarek_chaabane.pdf', '2018-01-25'),
    (6, 22, 'CV', 'CV_Nizar_Hamza.pdf', '/docs/cvs/nizar_hamza.pdf', '2019-03-05'),
    (7, 28, 'CV', 'CV_Zied_Tlili.pdf', '/docs/cvs/zied_tlili.pdf', '2018-05-25'),
    (8, 5, 'CV', 'CV_Youssef_Khelifi.pdf', '/docs/cvs/youssef_khelifi.pdf', '2022-01-05'),
    (9, 12, 'CV', 'CV_Hichem_Najar.pdf', '/docs/cvs/hichem_najar.pdf', '2020-07-10'),
    (10, 23, 'CV', 'CV_Asma_Chahed.pdf', '/docs/cvs/asma_chahed.pdf', '2020-05-20'),
    -- Contrats
    (11, 1, 'Contrat', 'Contrat_Mohamed_BenAli.pdf', '/docs/contracts/mohamed_benali.pdf', '2020-01-15'),
    (12, 2, 'Contrat', 'Contrat_Amira_Trabelsi.pdf', '/docs/contracts/amira_trabelsi.pdf', '2020-03-01'),
    (13, 3, 'Contrat', 'Contrat_Karim_Jebali.pdf', '/docs/contracts/karim_jebali.pdf', '2021-06-10'),
    (14, 11, 'Contrat', 'Contrat_Salma_Abidi.pdf', '/docs/contracts/salma_abidi.pdf', '2019-05-01'),
    (15, 16, 'Contrat', 'Contrat_Tarek_Chaabane.pdf', '/docs/contracts/tarek_chaabane.pdf', '2018-02-01'),
    (16, 22, 'Contrat', 'Contrat_Nizar_Hamza.pdf', '/docs/contracts/nizar_hamza.pdf', '2019-03-10'),
    (17, 28, 'Contrat', 'Contrat_Zied_Tlili.pdf', '/docs/contracts/zied_tlili.pdf', '2018-06-01'),
    (18, 6, 'Contrat', 'Contrat_CDD_Nadia_Gharbi.pdf', '/docs/contracts/nadia_gharbi.pdf', '2022-04-05'),
    (19, 14, 'Contrat', 'Contrat_CDD_Fares_Khiari.pdf', '/docs/contracts/fares_khiari.pdf', '2022-03-20'),
    (20, 20, 'Contrat', 'Contrat_CDD_Bilel_Gargouri.pdf', '/docs/contracts/bilel_gargouri.pdf', '2024-01-15'),

    -- Avenants
    (21, 1, 'Avenant', 'Avenant_Augmentation_2024_BenAli.pdf', '/docs/amendments/avenant_benali_2024.pdf', '2024-01-01'),
    (22, 2, 'Avenant', 'Avenant_Augmentation_2024_Trabelsi.pdf', '/docs/amendments/avenant_trabelsi_2024.pdf', '2024-01-01'),
    (23, 3, 'Avenant', 'Avenant_Promotion_Jebali.pdf', '/docs/amendments/avenant_jebali_promotion.pdf', '2024-07-01'),
    (24, 16, 'Avenant', 'Avenant_Augmentation_2024_Chaabane.pdf', '/docs/amendments/avenant_chaabane_2024.pdf', '2024-01-01'),
    (25, 28, 'Avenant', 'Avenant_Augmentation_2024_Tlili.pdf', '/docs/amendments/avenant_tlili_2024.pdf', '2024-01-01'),

    -- Fiches de paie (derniers mois)
    (26, 1, 'Fiche de paie', 'FichePaie_BenAli_2025_11.pdf', '/docs/payslips/benali_2025_11.pdf', '2025-11-30'),
    (27, 2, 'Fiche de paie', 'FichePaie_Trabelsi_2025_11.pdf', '/docs/payslips/trabelsi_2025_11.pdf', '2025-11-30'),
    (28, 3, 'Fiche de paie', 'FichePaie_Jebali_2025_11.pdf', '/docs/payslips/jebali_2025_11.pdf', '2025-11-30'),
    (29, 11, 'Fiche de paie', 'FichePaie_Abidi_2025_11.pdf', '/docs/payslips/abidi_2025_11.pdf', '2025-11-30'),
    (30, 16, 'Fiche de paie', 'FichePaie_Chaabane_2025_11.pdf', '/docs/payslips/chaabane_2025_11.pdf', '2025-11-30'),

    -- Attestations de travail
    (31, 5, 'Attestation', 'Attestation_Travail_Khelifi.pdf', '/docs/certificates/attestation_khelifi.pdf', '2025-03-15'),
    (32, 12, 'Attestation', 'Attestation_Travail_Najar.pdf', '/docs/certificates/attestation_najar.pdf', '2025-05-20'),
    (33, 23, 'Attestation', 'Attestation_Travail_Chahed.pdf', '/docs/certificates/attestation_chahed.pdf', '2025-07-10'),

    -- Certificats de formation
    (34, 3, 'Certification', 'Certification_Azure_Jebali.pdf', '/docs/certifications/azure_jebali.pdf', '2024-04-30'),
    (35, 4, 'Certification', 'Certification_AWS_Hamdi.pdf', '/docs/certifications/aws_hamdi.pdf', '2024-05-15'),
    (36, 19, 'Certification', 'Certification_CPA_Mansour.pdf', '/docs/certifications/cpa_mansour.pdf', '2024-06-20'),
    (37, 23, 'Certification', 'Certification_GoogleAds_Chahed.pdf', '/docs/certifications/google_chahed.pdf', '2025-01-25'),

    -- Évaluations annuelles
    (38, 1, 'Évaluation', 'Evaluation_2024_BenAli.pdf', '/docs/evaluations/eval_benali_2024.pdf', '2024-12-20'),
    (39, 2, 'Évaluation', 'Evaluation_2024_Trabelsi.pdf', '/docs/evaluations/eval_trabelsi_2024.pdf', '2024-12-20'),
    (40, 3, 'Évaluation', 'Evaluation_2024_Jebali.pdf', '/docs/evaluations/eval_jebali_2024.pdf', '2024-12-20'),
    (41, 11, 'Évaluation', 'Evaluation_2024_Abidi.pdf', '/docs/evaluations/eval_abidi_2024.pdf', '2024-12-20'),
    (42, 16, 'Évaluation', 'Evaluation_2024_Chaabane.pdf', '/docs/evaluations/eval_chaabane_2024.pdf', '2024-12-20'),
    (43, 22, 'Évaluation', 'Evaluation_2024_Hamza.pdf', '/docs/evaluations/eval_hamza_2024.pdf', '2024-12-20'),
    (44, 28, 'Évaluation', 'Evaluation_2024_Tlili.pdf', '/docs/evaluations/eval_tlili_2024.pdf', '2024-12-20'),

    -- Demandes de congés
    (45, 5, 'Demande congés', 'Conges_Ete_2025_Khelifi.pdf', '/docs/leaves/conges_khelifi_2025.pdf', '2025-05-10'),
    (46, 7, 'Demande congés', 'Conges_Ete_2025_Sassi.pdf', '/docs/leaves/conges_sassi_2025.pdf', '2025-05-15'),
    (47, 12, 'Demande congés', 'Conges_Ete_2025_Najar.pdf', '/docs/leaves/conges_najar_2025.pdf', '2025-04-20'),
    (48, 23, 'Demande congés', 'Conges_Ete_2025_Chahed.pdf', '/docs/leaves/conges_chahed_2025.pdf', '2025-06-05'),
    (49, 29, 'Demande congés', 'Conges_Ete_2025_Mkacher.pdf', '/docs/leaves/conges_mkacher_2025.pdf', '2025-06-10'),

    -- Pièces d'identité
    (50, 10, 'CIN', 'CIN_Souissi.pdf', '/docs/id/cin_souissi.pdf', '2025-03-12'),
    (51, 15, 'CIN', 'CIN_Rekik.pdf', '/docs/id/cin_rekik.pdf', '2024-06-01'),
    (52, 27, 'CIN', 'CIN_Arbi.pdf', '/docs/id/cin_arbi.pdf', '2025-01-15'),
    (53, 34, 'CIN', 'CIN_Dhouib.pdf', '/docs/id/cin_dhouib.pdf', '2025-06-01'),
    (54, 40, 'CIN', 'CIN_Aloui.pdf', '/docs/id/cin_aloui.pdf', '2025-05-15'),

    -- Diplômes
    (55, 2, 'Diplôme', 'Diplome_Ingenieur_Trabelsi.pdf', '/docs/diplomas/diplome_trabelsi.pdf', '2020-03-01'),
    (56, 3, 'Diplôme', 'Diplome_Licence_Jebali.pdf', '/docs/diplomas/diplome_jebali.pdf', '2021-06-10'),
    (57, 16, 'Diplôme', 'Diplome_Master_Finance_Chaabane.pdf', '/docs/diplomas/diplome_chaabane.pdf', '2018-02-01'),
    (58, 22, 'Diplôme', 'Diplome_MBA_Hamza.pdf', '/docs/diplomas/diplome_hamza.pdf', '2019-03-10'),
    (59, 45, 'Diplôme', 'Diplome_Doctorat_Bouslama.pdf', '/docs/diplomas/diplome_bouslama.pdf', '2018-09-01'),
    (60, 46, 'Diplôme', 'Diplome_Doctorat_Daoud.pdf', '/docs/diplomas/diplome_daoud.pdf', '2021-03-25');
SET IDENTITY_INSERT Documents OFF;

-- offres d'emploi

SET IDENTITY_INSERT JobOffers ON;
INSERT INTO JobOffers (JobOfferId, Title, Description, ContractType, DepartmentId, PositionId, PostDate, ExpiryDate, Status, CreatedDate, ModifiedDate)
VALUES
    -- Offres ouvertes
    (1, 'Développeur Full Stack Senior', 'Recherche développeur expérimenté .NET/React pour projets innovants',0 , 1, 2, '2025-11-01', '2025-01-31', 0, GETDATE(), GETDATE()),
    (2, 'DevOps Engineer', 'Expert infrastructure cloud et CI/CD pour modernisation',0 , 1, 5, '2025-11-15', '2025-02-15', 0, GETDATE(), GETDATE()),
    (3, 'Chargé de Recrutement', 'Profil dynamique pour renforcer l''équipe RH',0 , 2, 7, '2025-12-01', '2025-02-28', 0, GETDATE(), GETDATE()),
    (4, 'Contrôleur de Gestion Junior', 'Jeune diplômé finance pour analyse et reporting',0 , 3, 11, '2025-11-20', '2025-01-20', 0, GETDATE(), GETDATE()),
    (5, 'Digital Marketing Specialist', 'Expert SEO/SEM pour stratégie digitale',0 , 4, 13, '2025-12-05', '2025-02-05', 0, GETDATE(), GETDATE()),
    (6, 'Commercial BtoB', 'Expérience vente solutions IT entreprises', 0 ,5, 14, '2025-11-10', '2025-01-10', 0, GETDATE(), GETDATE()),
    -- Offres fermées (recrutements terminés)
    (7, 'Développeur Junior .NET', 'Poste pourvu - Youssef Khelifi recruté',1, 1, 1, '2021-11-01', '2021-12-31', 1, GETDATE(), GETDATE()),
    (8, 'Assistant RH', 'Poste pourvu - Marwa Rekik recrutée',1 , 2, 8, '2024-04-01', '2024-05-31', 1, GETDATE(), GETDATE()),
    (9, 'Comptable Senior', 'Poste pourvu - Ramzi Zouari recruté',1 , 3, 10, '2020-09-01', '2020-10-31', 1, GETDATE(), GETDATE()),
    (10, 'Responsable Logistique', 'Poste pourvu - Amel Chebbi recrutée',1 , 7, 17, '2021-08-01', '2021-09-30', 1, GETDATE(), GETDATE()),
    (11, 'Ingénieur Qualité', 'Poste pourvu - Imen Mathlouthi recrutée',1 , 8, 18, '2022-10-01', '2022-11-30', 1, GETDATE(), GETDATE()),
    (12, 'Support Technique', 'Poste pourvu - Nabil Ghariani recruté',1 , 10, 20, '2024-06-01', '2024-07-31', 1, GETDATE(), GETDATE());
SET IDENTITY_INSERT JobOffers OFF;

-- Candidates
SET IDENTITY_INSERT Candidates ON;
INSERT INTO Candidates (CandidateId, FirstName, LastName, Gender, DateOfBirth, Email, Phone, Address, CVPath, JobOfferId, Status, ApplicationDate)
VALUES
    -- Candidats pour Développeur Full Stack (offre 1)
    (1, 'Amine', 'Fourati', 1,'1996-03-12', 'amine.fourati@email.com', '98765432', '15 Rue de la Paix, Tunis', '/cvs/amine_fourati.pdf', 1, 0, '2025-11-05'),
    (2, 'Yasmine', 'Ayari', 2,'1997-07-25', 'yasmine.ayari@email.com', '97654321', '28 Avenue Habib Bourguiba, La Marsa', '/cvs/yasmine_ayari.pdf', 1, 1, '2025-11-08'),
    (3, 'Khalil', 'Mbarek', 1,'1995-01-18', 'khalil.mbarek@email.com', '96543210', '42 Rue Ibn Khaldoun, Sousse', '/cvs/khalil_mbarek.pdf', 1, 2, '2025-11-12'),
    (4, 'Nour', 'Sakli',2, '1998-09-03', 'nour.sakli@email.com', '95432109', '17 Avenue de France, Tunis', '/cvs/nour_sakli.pdf', 1, 3, '2025-11-15'),

    -- Candidats pour DevOps (offre 2)
    (5, 'Faker', 'Jedidi', 1,'1994-11-20', 'faker.jedidi@email.com', '94321098', '33 Rue de Marseille, Sfax', '/cvs/faker_jedidi.pdf', 2, 0, '2025-11-18'),
    (6, 'Sabrine', 'Chouchane', 2,'1996-05-14', 'sabrine.chouchane@email.com', '93210987', '56 Avenue Mohamed V, Tunis', '/cvs/sabrine_chouchane.pdf', 2, 1, '2025-11-20'),
    (7, 'Rami', 'Ouerhani', 1,'1993-08-09', 'rami.ouerhani@email.com', '92109876', '21 Rue de Rome, Bizerte', '/cvs/rami_ouerhani.pdf', 2, 0, '2025-11-22'),

    -- Candidats pour Chargé RH (offre 3)
    (8, 'Amina', 'Ltaief', 2,'1997-02-27', 'amina.ltaief@email.com', '91098765', '44 Avenue de Carthage, Tunis', '/cvs/amina_ltaief.pdf', 3, 0, '2025-12-02'),
    (9, 'Seif', 'Jlassi', 1,'1995-06-30', 'seif.jlassi@email.com', '90987654', '19 Rue Charles de Gaulle, Monastir', '/cvs/seif_jlassi.pdf', 3, 1, '2025-12-04'),
    (10, 'Emna', 'Mahjoub', 2,'1998-10-11', 'emna.mahjoub@email.com', '89876543', '37 Avenue Habib Thameur, Tunis', '/cvs/emna_mahjoub.pdf', 3, 0, '2025-12-06'),

    -- Candidats pour Contrôleur Gestion (offre 4)
    (11, 'Aymen', 'Agrebi', 1,'1992-12-05', 'aymen.agrebi@email.com', '88765432', '52 Rue de la République, Tunis', '/cvs/aymen_agrebi.pdf', 4, 0, '2025-11-25'),
    (12, 'Meriem', 'Kchaou', 2,'1994-04-16', 'meriem.kchaou@email.com', '87654321', '14 Avenue de la Liberté, Sousse', '/cvs/meriem_kchaou.pdf', 4, 2, '2025-11-27'),
    (13, 'Walid', 'Driss', 1,'1991-09-22', 'walid.driss@email.com', '86543210', '29 Rue Ibn Sina, Tunis', '/cvs/walid_driss.pdf', 4, 0, '2025-11-30'),

    -- Candidats pour Marketing Digital (offre 5)
    (14, 'Safa', 'Barhoumi', 2,'1999-01-08', 'safa.barhoumi@email.com', '85432109', '63 Avenue de Paris, La Marsa', '/cvs/safa_barhoumi.pdf', 5, 0, '2025-12-07'),
    (15, 'Tarek', 'Chaari', 1,'1993-03-19', 'tarek.chaari@email.com', '84321098', '18 Rue de la Kasbah, Tunis', '/cvs/tarek_chaari.pdf', 5, 1, '2025-12-09'),
    (16, 'Dorra', 'Mhenni', 2,'1997-07-02', 'dorra.mhenni@email.com', '83210987', '41 Avenue Bourguiba, Sfax', '/cvs/dorra_mhenni.pdf', 5, 0, '2025-12-11'),

    -- Candidats pour Commercial BtoB (offre 6)
    (17, 'Montassar', 'Hachicha', 1,'1990-05-26', 'montassar.hachicha@email.com', '82109876', '25 Rue Charles Nicolle, Tunis', '/cvs/montassar_hachicha.pdf', 6, 0, '2025-11-13'),
    (18, 'Hanene', 'Cheikhrouhou', 2,'1994-08-17', 'hanene.cheikhrouhou@email.com', '81098765', '58 Avenue Mohamed V, Bizerte', '/cvs/hanene_cheikhrouhou.pdf', 6, 2, '2025-11-16'),
    (19, 'Kamel', 'Belkhiria', 1,'1989-11-01', 'kamel.belkhiria@email.com', '80987654', '32 Rue de Marseille, Tunis', '/cvs/kamel_belkhiria.pdf', 6, 1, '2025-11-19'),
    (20, 'Wided', 'Omrane', 2,'1996-06-13', 'wided.omrane@email.com', '79876543', '47 Avenue de France, Monastir', '/cvs/wided_omrane.pdf', 6, 0, '2025-11-21'),

    -- Anciens candidats
    (21, 'Youssef', 'Khelifi', 1,'1988-02-10', 'youssef.khelifi@email.com', '78765432', '56 Rue de Marseille, Sfax', '/cvs/youssef_khelifi_old.pdf', 7, 3, '2021-11-10'),
    (22, 'Marwa', 'Rekik', 2,'1990-09-28', 'marwa.rekik@email.com', '77654321', '34 Rue Ibn Sina, Tunis', '/cvs/marwa_rekik_old.pdf', 8, 4, '2024-04-15'),
    (23, 'Imen', 'Mathlouthi', 2,'1992-12-14', 'imen.mathlouthi@email.com', '76543210', '23 Avenue de Carthage, Sousse', '/cvs/imen_mathlouthi_old.pdf', 11, 3, '2022-10-20'),
    (24, 'Nabil', 'Ghariani', 1,'1987-07-07', 'nabil.ghariani@email.com', '75432109', '34 Rue de Rome, Tunis', '/cvs/nabil_ghariani_old.pdf', 12, 3, '2024-06-18'),

    -- Candidat refusé
    (25, 'Sofiene', 'Marzouki', 1,'1995-04-04', 'sofiene.marzouki@email.com', '74321098', '62 Avenue Bourguiba, Tunis', '/cvs/sofiene_marzouki.pdf', 1, 3, '2025-11-10');
SET IDENTITY_INSERT Candidates OFF;

-- Interviews
SET IDENTITY_INSERT Interviews ON;
INSERT INTO Interviews (InterviewId, CandidateId, InterviewDate, Location, InterviewerName, Notes, Result, CreatedDate)
VALUES
    -- Entretiens terminés avec résultats
    (1, 2, '2025-11-15', 'Salle 201', 'Mohamed Ben Ali', 'Très bon profil technique, excellente communication', 2, GETDATE()),
    (2, 3, '2025-11-18', 'Salle 201', 'Mohamed Ben Ali', 'Compétences techniques solides, manque expérience cloud', 3, GETDATE()),
    (3, 4, '2025-11-20', 'Salle 201', 'Amira Trabelsi', 'Expert architecture, profil senior confirmé', 2, GETDATE()),
    (4, 6, '2025-11-25', 'Salle 103', 'Mohamed Ben Ali', 'Excellente maîtrise DevOps, certifications AWS', 2, GETDATE()),
    (5, 9, '2025-12-08', 'Salle 205', 'Salma Abidi', 'Profil dynamique, bonne connaissance recrutement', 2, GETDATE()),
    (6, 12, '2025-12-02', 'Salle 303', 'Tarek Chaabane', 'Bon niveau technique, accepté en stage', 2, GETDATE()),
    (7, 15, '2025-12-12', 'Salle 404', 'Nizar Hamza', 'Expert SEO/SEM, portefeuille impressionnant', 2, GETDATE()),
    (8, 18, '2025-11-22', 'Salle 505', 'Zied Tlili', 'Excellente expérience BtoB, accepté', 2, GETDATE()),
    (9, 19, '2025-11-24', 'Salle 505', 'Zied Tlili', 'Bon profil commercial, entretien positif', 2, GETDATE()),
    (10, 25, '2025-11-17', 'Salle 201', 'Mohamed Ben Ali', 'Compétences insuffisantes pour le poste', 1, GETDATE()),
    -- Entretiens programmés (en attente)
    (11, 1, '2025-01-10', 'Salle 201', 'Mohamed Ben Ali', 'Premier entretien technique', 0, GETDATE()),
    (12, 5, '2025-01-12', 'Salle 103', 'Lina Bouaziz', 'Entretien technique DevOps', 0, GETDATE()),
    (13, 7, '2025-01-15', 'Salle 103', 'Mohamed Ben Ali', 'Second entretien DevOps', 0, GETDATE()),
    (14, 8, '2025-01-18', 'Salle 205', 'Salma Abidi', 'Entretien RH initial', 0, GETDATE()),
    (15, 10, '2025-01-20', 'Salle 205', 'Hichem Najar', 'Second entretien RH', 0, GETDATE()),
    (16, 11, '2025-01-14', 'Salle 303', 'Tarek Chaabane', 'Entretien contrôleur gestion', 0, GETDATE()),
    (17, 13, '2025-01-16', 'Salle 303', 'Samia Boussetta', 'Second entretien finance', 0, GETDATE()),
    (18, 14, '2025-01-22', 'Salle 404', 'Nizar Hamza', 'Entretien marketing digital', 0, GETDATE()),
    (19, 16, '2025-01-24', 'Salle 404', 'Asma Chahed', 'Test pratique marketing', 0, GETDATE()),
    (20, 20, '2025-01-17', 'Salle 505', 'Zied Tlili', 'Entretien commercial BtoB', 0, GETDATE());
SET IDENTITY_INSERT Interviews OFF;

-- Promotions
SET IDENTITY_INSERT Promotions ON;
INSERT INTO Promotions (PromotionId, EmployeeId, OldPositionId, NewPositionId, OldSalary, NewSalary, PromotionDate, Justification, CreatedDate)
VALUES
    -- Promotions 2024
    (1, 3, 1, 2, 4500, 5200, '2024-07-01', 'Excellentes performances, passage senior après 2 ans', GETDATE()),
    (2, 5, 1, 1, 2500, 2800, '2024-12-15', 'Augmentation mérite première année', GETDATE()),
    (3, 12, 8, 7, 3200, 3800, '2024-06-01', 'Promotion chargé de recrutement', GETDATE()),
    (4, 18, 10, 10, 4000, 4600, '2024-10-01', 'Augmentation performance comptable', GETDATE()),
    (5, 24, 13, 13, 3800, 4300, '2024-09-01', 'Reconnaissance résultats marketing', GETDATE()),
    -- Promotions 2025
    (6, 4, 1, 2, 4200, 5000, '2025-01-01', 'Passage senior développeur', GETDATE()),
    (7, 7, 1, 1, 2600, 2900, '2025-01-01', 'Augmentation annuelle junior', GETDATE()),
    (8, 8, 5, 5, 5000, 5500, '2025-01-01', 'Augmentation DevOps performance', GETDATE()),
    (9, 13, 7, 7, 3200, 3600, '2025-01-01', 'Augmentation chargé RH', GETDATE()),
    (10, 19, 11, 11, 4500, 5200, '2025-01-01', 'Augmentation contrôleur gestion', GETDATE()),
    (11, 21, 11, 11, 4400, 4900, '2025-02-01', 'Augmentation contrôleur gestion', GETDATE()),
    (12, 26, 13, 13, 3700, 4100, '2025-01-01', 'Augmentation specialist marketing', GETDATE()),
    (13, 30, 14, 14, 3500, 4000, '2025-01-01', 'Augmentation commercial performance', GETDATE()),
    (14, 33, 14, 14, 3400, 3800, '2025-01-01', 'Augmentation commercial', GETDATE()),
    (15, 46, 19, 19, 5500, 6200, '2025-01-01', 'Augmentation chercheur R&D', GETDATE());
SET IDENTITY_INSERT Promotions OFF;
