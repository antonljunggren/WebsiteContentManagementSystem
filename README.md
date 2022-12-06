# Anton Ljunggren's Website - Content Management System <!-- omit in toc -->

This is a tailored content management system for use by my website.
At the start it will contain photographs.
These will be easy to create, edit, update and retrieve via a api

<br>

## Table of contents <!-- omit in toc -->
- [About](#about)
- [Current technical state](#current-technical-state)

<br>

## About
This project is made to handle the content on my website.
Right now it is just film photographs, but the plan is to also include articles, image resources and much more..

The most important point of this project is for me to learn more .NET, C#, Azure Services and system architecture, like Clean Architecture, EF Core, CQRS...

To begin, this project is intentionally very basic in it's structure.
Mostly to document the journey of a small project to a larger system.
The plan is to end up using concepts like Clean Architecture, CQRS, DDD where applicable.

And I also did that: created a system with the same features as this, but rigorously using the above concepts.
It beacame clear to me that i took away the focus from the core idea and instead put all the energy on implementing all the concepts.

So the intention is for this project to grow over time, both technicaly and feature -wise.

## Current technical state
- Azure ComsoDb for persistant document storage
- Azure Blob Storage for file storage
- EF Core as ORM
- ASP .NET Core as frontend
- ImageSharp for image processing
- Azure Function for data retrieval via Azure Api Management 