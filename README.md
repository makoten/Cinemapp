# Cinemapp

![absolute cinema](https://media.tenor.com/w5jeyHGonH0AAAAe/absolute-cinema-meme.png)

## About

A RESTful API made with C# and love!

## Design Decisions & features

### Movies.Api
Contains all public-facing endpoints, and related functionality (mappings, etc.)

### Movies.Application
Business Logic and Repositories. 
Dapper was used instead of Entity Core. (pending)

### Movies.Contracts
To be exported into a Nuget Package as an SDK (pending)