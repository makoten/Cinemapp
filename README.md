# Cinemapp

![absolute cinema](https://media.tenor.com/w5jeyHGonH0AAAAe/absolute-cinema-meme.png)

## About

A RESTful API made with C# and love!

## Features
- Ability to CRUD Movies and Movie Ratings (authentication may be required)
- Dockerized PostgreSQL
- JWT Authentication and Authorization
- Documented Swagger
- Health check

## Design Decisions & features

### Movies.Api
CRUD for Movies and Ratings

### Movies.Application
Business Logic and Repositories. 
Dapper was used instead of Entity Core. (pending)

### Movies.Contracts
To be exported into a Nuget Package as an SDK (pending)


### JWT Generation
For the purposes of Authentication and Authorization, this API only assumes and verifies JSON Web Tokens. It does not generate them!