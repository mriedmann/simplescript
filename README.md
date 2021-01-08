
# ILV Microservice Architectures - Project 2 Customer Service

## Authors

* Michael RIEDMANN <se19m020@technikum-wien.at>
* Patrick SENGEIS <se19m015@technikum-wien.at>
* Patrick WAGNER <se19m022@technikum-wien.at>

---

## Requirements Customer Service

> Taken from 'Projekt 2 Aufgabenstellung'

Das CustomerService verwaltet die Kundendaten. In diesem Zusammenhang werden passende Registrierung/Login Commands/Events angeboten. Folgende Operationen sollen implementiert
werden:

### 1. Registration

Mit diesem Command kann man einen Kunden registrieren. Dieser besteht aus den folgenden Daten:

* Vorname
* Nachname
* Geburtsdatum
* Handynummer
* Benutzername
* Passwort

Im Zuge der Registrierung wird ihm eine ID zugewiesen.

### 2. Login

Mit diesem Command kann sich ein Kunde durch Angabe von Passwort und Benutzername beim System anmelden. Als Resultat des Logins wird dem Benutzer eine Session (e.g.: Hash, Guid, Int) zugewiesen. Die Gültigkeit der Session beträgt 30 Minuten.

### 3. CheckSession

Mit diesem Command kann man die Gültigkeit einer Session überprüfen. Bei jedem Aufruf wird die Session um 30 Minuten verlängert. Registrierte Benutzer werden über eine entsprechende Database Repository Implementierung persistiert.

## Project description

The structure is leaned on the framework (similar to SellTicketService).
exampleservice.CustomerService is the main entry point of this service. It contains handle methods for three commands to fullfill the requirements:

* RegisterCustomerCommand
* LoginCommand
* CheckSessionCommand

Each command is redirected to a corresponding handler (package Handler) and is only responsible for one command (SRP). All Handlers are derived from CustomerHandlerBase. The Handle method verifies the given command input arguments and executes / orchestrates the business logic via procedures and publishes events to the message bus.

Error Handling is done by sending positive or negative events via the framework´s EventBase. Each positive or negative event is distinguished by the function of the service command. Furthermore, argument verfication in negative cases follows into several exceptions.

All handlers implements their GetProcedure. The procedure description is done with single steps (package Steps). Steps using data repositories, utils and manages the state of the CustomerContext. For this student project, no DTO´s are used and data is only managed in-memory.

The package contract contains commands, events and models, which are used by the service to describe it´s contract.

Unit-Tests are separated between handlers and steps. They cover the business logics as well as expected positive and negative events.
