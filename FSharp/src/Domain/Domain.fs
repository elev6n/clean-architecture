namespace CleanArchitecture.Domain

open System

module Exceptions =
    type DomainException(message: string) =
        inherit Exception(message)

    type ValidationException(message: string) =
        inherit Exception(message)

    type NotFoundException(message: string) =
        inherit Exception(message)

    type ConflictException(message: string) =
        inherit Exception(message)

module Common =
    type EntityId = EntityId of Guid
    type Email = Email of string
    type Username = Username of string

open Common

module User =
    [<CLIMutable>]
    type User =
        { Id: EntityId
          Email: Email
          Username: Username
          CreatedAt: DateTime
          UpdatedAt: DateTime }

    type CreateUserRequest = { Email: Email; Username: Username }

open User

module Todo =
    type TodoStatus =
        | Pending
        | InProgress
        | Completed
        | Archived

    [<CLIMutable>]
    type Todo =
        { Id: EntityId
          Title: string
          Description: string option
          Status: TodoStatus
          UserId: EntityId
          CreatedAt: DateTime
          UpdatedAt: DateTime }

    type CreateTodoRequest =
        { Title: string
          Description: string option
          UserId: EntityId }

    type UpdateTodoRequest =
        { Id: EntityId
          Title: string option
          Description: string option
          Status: TodoStatus option }

open Todo

module RepositoryInterfaces =
    type IUserRepository =
        abstract GetById: EntityId -> Async<User option>
        abstract GetByEmail: Email -> Async<User option>
        abstract Create: CreateUserRequest -> Async<User>
        abstract GetAll: unit -> Async<User list>

    type ITodoRepository =
        abstract GetById: EntityId -> Async<Todo option>
        abstract GetByUserId: EntityId -> Async<Todo list>
        abstract Create: CreateTodoRequest -> Async<Todo>
        abstract Update: UpdateTodoRequest -> Async<Todo option>
        abstract Delete: EntityId -> Async<bool>
