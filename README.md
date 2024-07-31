# Lab Instrument Booking

### Short description

A C#/.NET 8 project developed to coordinate and automate recording of laboratory instrument usage using third-party APIs to process data proprietary data files and HTTP booking system end points.

### Languages used

- C#

### Overview

This script was intially developed in my previous role to coordinate recording of instrument usage on our highest throughput laboratory instrument. The app has been completely refactored using an object-oriented approach with test driven development using xUnit and Moq, where needed.

Instrument vendor libraries are used to access proprietary data files to determine acquisition times and overall project acquisition length. Booking system API endpoints are exploited to determine user id, applicable charge rate and account code before placing a booking based on actual isntrument usage. A lightweight API was separately developed to replicate the API of the previously used commercial booking system.

### Future Features

- Additional IBooking imlpementations for development and QC acquisitions
- Add data file back up feature to archive recorded data on remote data repository
- Additional IInstrument implementation for different vendor instruments

