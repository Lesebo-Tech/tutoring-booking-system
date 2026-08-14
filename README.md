# Tutoring Booking System

A booking system built to solve a real problem I was having running a tutoring business over WhatsApp. I kept losing track of who needed a session, when, and whether they'd been confirmed. This app replaces that chaos with a proper booking flow: students pick from available time slots, bookings get tracked with a status, and double bookings are prevented at the data level.

## Why I built this

Running tutoring sessions via WhatsApp meant everything lived in scattered chats. Requests got missed, slots got double-booked, and I had no clean way to see who was confirmed for a given week. This project is my attempt to fix that with a proper system instead of a messaging thread.

## Tech Stack

- Backend: ASP.NET MVC (C#)
- Database: SQL Server, accessed via Entity Framework (Code First)
- Frontend: Razor Views, HTML, CSS, JavaScript

## Features

- Student profiles (name, contact, subject, grade)
- Available time slot management
- Booking requests with status tracking (Pending / Confirmed / Completed / Cancelled)
- Conflict prevention — a slot can't be double-booked
- Payment status tracking (Paid / Unpaid)
