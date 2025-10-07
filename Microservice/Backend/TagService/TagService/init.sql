CREATE DATABASE IF NOT EXISTS tag_service_db;
USE tag_service_db;

CREATE TABLE IF NOT EXISTS tags (
    tag_id INT AUTO_INCREMENT PRIMARY KEY,
    tag_name VARCHAR(50) NOT NULL,
    color VARCHAR(7) DEFAULT '#808080' COMMENT 'Hex color',
    cognito_sub VARCHAR(50) NOT NULL COMMENT 'Creator',
    group_id INT NULL COMMENT 'NULL = personal tag, NOT NULL = group tag',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    INDEX idx_cognito_sub (cognito_sub),
    INDEX idx_group_id (group_id),
    UNIQUE KEY unique_tag_per_scope (tag_name, cognito_sub, group_id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS todo_tags (
    todo_id INT NOT NULL,
    tag_id INT NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (todo_id, tag_id),
    INDEX idx_todo_id (todo_id),
    INDEX idx_tag_id (tag_id),
    FOREIGN KEY (tag_id) REFERENCES tags(tag_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
