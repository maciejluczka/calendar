-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: May 23, 2025 at 10:01 PM
-- Server version: 10.4.28-MariaDB
-- PHP Version: 8.2.4

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `calendar`
--

-- --------------------------------------------------------

--
-- Table structure for table `eventsdsadas`
--

CREATE TABLE `eventsdsadas` (
  `date` varchar(20) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `eventstest`
--

CREATE TABLE `eventstest` (
  `date` varchar(20) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `eventstest`
--

INSERT INTO `eventstest` (`date`, `name`, `description`) VALUES
('15.5.2025', 'testdsadasd', 'eventsdsadasdsaddadasdasdsa'),
('5.6.2025', 'dsadasd', 'eventsfasddasd'),
('1.5.2025', '', 'events'),
('1.5.2025', '', 'events'),
('1.5.2025', '', 'events'),
('1.5.2025', '', 'eventsadsad'),
('2.5.2025', '', 'eventsdsadasfaf');

-- --------------------------------------------------------

--
-- Table structure for table `eventstest1`
--

CREATE TABLE `eventstest1` (
  `date` varchar(20) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `eventstest1`
--

INSERT INTO `eventstest1` (`date`, `name`, `description`) VALUES
('2.5.2025', 'naa', 'dsadsadsadsadsadsadsadsadsadsadsadsadsadsadsadsadsadsadsadsadsadsadsadsadsadsa'),
('3.5.2025', 'dsa', 'dsadas'),
('11.5.2025', '', 'dsadfasdfasdfsadd'),
('9.5.2025', '', ''),
('10.5.2025', 'dasda', 'dsadasd');

-- --------------------------------------------------------

--
-- Table structure for table `eventstest123`
--

CREATE TABLE `eventstest123` (
  `date` varchar(20) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `eventstest123`
--

INSERT INTO `eventstest123` (`date`, `name`, `description`) VALUES
('9.5.2025', 'Urodziny', 'dsafasfdf');

-- --------------------------------------------------------

--
-- Table structure for table `eventstestdsad`
--

CREATE TABLE `eventstestdsad` (
  `date` varchar(20) DEFAULT NULL,
  `name` varchar(255) DEFAULT NULL,
  `description` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `users`
--

CREATE TABLE `users` (
  `id` int(11) NOT NULL,
  `login` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `eventsTable` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `users`
--

INSERT INTO `users` (`id`, `login`, `password`, `eventsTable`) VALUES
(14, 'test', '098f6bcd4621d373cade4e832627b4f6', 'eventstest'),
(15, 'test1', 'cc03e747a6afbbcbf8be7668acfebee5', 'eventstest1'),
(16, 'dsadas', '89defae676abd3e3a42b41df17c40096', 'eventsdsadas'),
(17, 'testdsad', '5676e37338d668578a3686ab581abe0f', 'eventstestdsad'),
(18, 'test123', 'c06db68e819be6ec3d26c6038d8e8d1f', 'eventstest123');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `users`
--
ALTER TABLE `users`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `users`
--
ALTER TABLE `users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=19;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
