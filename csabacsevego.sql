-- phpMyAdmin SQL Dump
-- version 5.2.0
-- https://www.phpmyadmin.net/
--
-- Gép: 127.0.0.1
-- Létrehozás ideje: 2024. Ápr 16. 12:26
-- Kiszolgáló verziója: 10.4.24-MariaDB
-- PHP verzió: 8.1.6

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Adatbázis: `csabacsevego`
--
CREATE DATABASE IF NOT EXISTS `csaba` DEFAULT CHARACTER SET utf8 COLLATE utf8_hungarian_ci;
USE `csaba`;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `friends`
--

CREATE TABLE `friends` (
  `user_id` int(20) NOT NULL,
  `friend_id` int(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `messages`
--

CREATE TABLE `messages` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `sender_id` int(20) NOT NULL,
  `reciever` varchar(20) NOT NULL,
  `message` varchar(2000) COLLATE utf8_hungarian_ci NOT NULL,
  `created_at` datetime NOT NULL,
  `updated_at` datetime NOT NULL,
  `deleted_at` datetime NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

-- --------------------------------------------------------

--
-- Tábla szerkezet ehhez a táblához `users`
--

CREATE TABLE `users` (
  `id` int(20) NOT NULL AUTO_INCREMENT,
  `username` varchar(20) COLLATE utf8_hungarian_ci NOT NULL,
  `email` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `password` varchar(255) COLLATE utf8_hungarian_ci NOT NULL,
  `created_at` datetime DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL,
  `deleted_at` datetime DEFAULT NULL,
  `active` tinyint(1) NOT NULL DEFAULT 0,
  `censured` tinyint(1) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;

CREATE TABLE `servers` (
  `user_id` int(20) NOT NULL,
  `server_address` VARCHAR(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_hungarian_ci;
--
-- Indexek a kiírt táblákhoz
--

--
-- A tábla indexei `friends`
--
ALTER TABLE `friends`
  ADD PRIMARY KEY (`user_id`);
ALTER TABLE `servers`
  ADD PRIMARY KEY (`user_id`);
--
-- A tábla indexei `messages`
--
ALTER TABLE `messages`
  ADD KEY `user_id` (`sender_id`);

--
-- A tábla indexei `users`
--
ALTER TABLE `users`
  ADD UNIQUE KEY `username` (`username`),
  ADD UNIQUE KEY `email` (`email`);

--
-- A kiírt táblák AUTO_INCREMENT értéke
--

--
-- AUTO_INCREMENT a táblához `messages`
--
ALTER TABLE `messages`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT a táblához `users`
--
ALTER TABLE `users`
  MODIFY `id` int(20) NOT NULL AUTO_INCREMENT;

--
-- Megkötések a kiírt táblákhoz
--

--
-- Megkötések a táblához `friends`
--
ALTER TABLE `friends`
  ADD CONSTRAINT `friends_ibfk_1` FOREIGN KEY (`user_id`) REFERENCES `users` (`id`);

--
-- Megkötések a táblához `messages`
--
ALTER TABLE `messages`
  ADD CONSTRAINT `messages_ibfk_1` FOREIGN KEY (`sender_id`) REFERENCES `users` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
