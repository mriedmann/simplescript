# ExampleService: Customer Service

> Das CustomerService verwaltet die Kundendaten. In diesem Zusammenhang werden passende Registrierung/Login Commands/Events angeboten. 

## Requirements

Following processes/commands should be implemented.

### 1. Registration

> Mit diesem Command kann man einen Kunden registrieren. Dieser besteht aus den folgenden Daten:
a. Vorname
b. Nachname
c. Geburtsdatum
d. Handynummer
e. Benutzername
f. Passwort
Im Zuge der Registrierung wird ihm eine ID zugewiesen.

### 2. Login

> Mit diesem Command kann sich ein Kunde durch Angabe von Passwort und Benutzername beim System anmelden. Als Resultat des Logins wird dem Benutzer eine Session (e.g.: Hash, Guid, Int) zugewiesen. Die Gültigkeit der Session beträgt 30 Minuten.

### 3. CheckSession

> Mit diesem Command kann man die Gültigkeit einer Session überprüfen. Bei jedem Aufruf wird die Session um 30 Minuten verlängert. Registrierte Benutzer werden über eine entsprechende Database Repository Implementierung persistiert.

## Ideas

### Service Segregation / Single-Responsibility-Principle

Currently, the Customer-Service is doing Authentication (Login), User-Registration and Session-Handling/Validation. It should be very easy to separate at least the Login/Session part from User-Registration.

## Multiple Handlers

The example service only has one handler method, we now use 3 (one per command). We could try to find a better architecture using some kind of composition pattern to make (hypothetical) refactoring into multiple services much easier.

### DTO

We are currently using the same objects for communication (inside events) and for persistence. This makes validation and minimal information disclosure hard. This 'mixed' pattern is also in the SellTicketService, we could nevertheless introduce DTO or ViewModel-like classes to refactor this.

### Test-Coverage

With ~60% Branch coverage we are far from perfect. We could try to do some refactoring to reduce branches. Adding more tests could also be valid, but we should not overextend the specification. There are no specific requirements on error-handling. Maybe we can just remove some checks or ask for more information on how to handle edge-cases.
