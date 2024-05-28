-- Create additional users
CREATE USER keycloak WITH PASSWORD 'keycloak';
CREATE USER proxyadm WITH PASSWORD 'proxyadm';

-- Create additional databases
CREATE DATABASE keycloak OWNER = keycloak;
CREATE DATABASE proxy OWNER = proxyadm;

-- Grant privileges
GRANT ALL PRIVILEGES ON DATABASE keycloak TO keycloak;
GRANT ALL PRIVILEGES ON DATABASE proxy TO proxyadm;
