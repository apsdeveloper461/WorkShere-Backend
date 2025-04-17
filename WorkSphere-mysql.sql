show databases;
create database WorkSphere;
 use WorkSphere;
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    password VARCHAR(50) NOT NULL,
    role ENUM('developer','admin', 'product manager') NOT NULL,
    name VARCHAR(100),
    email VARCHAR(100) unique,
    activationStatus BOOLEAN,
    workingStatus BOOLEAN default FALSE
);

INSERT INTO users (password, role, name, email, activationStatus) VALUES
('admin','admin',"admin","admin@gmail.com",TRUE),
( 'password1', 'developer', 'User One', 'user1@example.com', TRUE),
('password2', 'product manager', 'User Two', 'user2@example.com', TRUE),
('password3', 'developer', 'User Three', 'user3@example.com', FALSE),
('password4', 'product manager', 'User Four', 'user4@example.com', TRUE),
('password5', 'developer', 'User Five', 'user5@example.com', FALSE);



CREATE TABLE projects (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    status boolean,
    startDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    endDate DATETIME
);


CREATE TABLE project_users (
    projectId INT,
    userId INT,
    PRIMARY KEY (projectId, userId),
    FOREIGN KEY (projectId) REFERENCES projects(id) ON DELETE CASCADE,
    FOREIGN KEY (userId) REFERENCES users(id) ON DELETE CASCADE
);


CREATE TABLE feedback (
    id INT AUTO_INCREMENT PRIMARY KEY,
    senderId INT NOT NULL,
    projectId INT NOT NULL,
    message TEXT NOT NULL,
    time DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (senderId) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (projectId) REFERENCES projects(id) ON DELETE CASCADE
);

CREATE TABLE timeLog (
    id INT AUTO_INCREMENT PRIMARY KEY,
    projectId INT NOT NULL,
    developerId INT NOT NULL,
    description TEXT NOT NULL,
    workedHours FLOAT NOT NULL,
    status ENUM('pending', 'approved', 'reject') NOT NULL,
    date DATE NOT NULL,
    FOREIGN KEY (projectId) REFERENCES projects(id) ON DELETE CASCADE,
    FOREIGN KEY (developerId) REFERENCES users(id) ON DELETE CASCADE
);


CREATE TABLE finance (
    id INT AUTO_INCREMENT PRIMARY KEY,
    projectId INT NOT NULL,
    hourly_rate FLOAT NOT NULL,
    managementCost FLOAT NOT NULL,
    created_date DATETIME DEFAULT NOW(),
    FOREIGN KEY (projectId) REFERENCES projects(id) ON DELETE CASCADE
);


