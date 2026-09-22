- RaceDay Database Script
-- PROG6212 - Programming 2B
-- Liam Scott - ST10467183

-- Drop the database if it already exists
IF DB_ID('RaceDayDB') IS NOT NULL
    DROP DATABASE RaceDayDB;
GO

-- Create the database
CREATE DATABASE RaceDayDB;
GO

-- Use the database
USE RaceDayDB;
GO