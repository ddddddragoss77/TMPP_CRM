# Explicație Implementare Principii OOP și SOLID

Acest document descrie modul în care principiile OOP și SOLID au fost aplicate în proiectul CRM.

## 1. Principiile OOP (Programare Orientată pe Obiecte)

### Încapsulare (Encapsulation)
Ascunderea detaliilor de implementare și protejarea stării obiectelor.
*   **Implmentare**: 
    *   Entitățile precum `Lead` (`Domain/Entities/Lead.cs`) folosesc proprietăți publice cu `get` și `set` pentru acces controlat. 
    *   Implementarea internă a `InMemoryRepository` (`Infrastructure/Persistence/InMemoryRepository.cs`) ascunde dicționarul `_store` făcându-l `private`, expunând datele doar prin metodele publice `Add`, `Get`, etc.

### Moștenire (Inheritance)
Reutilizarea codului prin derivarea claselor.
*   **Implmentare**: 
    *   Clasa `BaseEntity` (`Domain/Common/BaseEntity.cs`) este moștenită de toate entitățile (`Lead`, `Client`, `Deal`), oferind automat proprietățile `Id` și `CreatedAt` tuturor.
    *   `CallActivity` și `EmailActivity` moștenesc clasa abstractă `Activity` (`Domain/Entities/Activity.cs`), preluând proprietățile comune (`DueDate`, `Description`).

### Polimorfism (Polymorphism)
Capacitatea obiectelor de a lua mai multe forme.
*   **Implmentare**: 
    *   Repository-ul este definit generic `IRepository<T>` (`Domain/Interfaces/IRepository.cs`). Codul din `LeadService` lucrează cu `IRepository<Lead>`, dar la runtime primește `InMemoryRepository<Lead>`.
    *   Putem trata `CallActivity` și `EmailActivity` ca obiecte de tip `Activity` (ex: într-o listă de activități).

### Abstractizare (Abstraction)
Definirea unor modele conceptuale fără detalii de implementare.
*   **Implmentare**: 
    *   Interfața `IRepository<T>` definește *ce* poate face un repository (Add, Get, Update, Delete), ascunzând complet *cum* se face asta (In-Memory, SQL, citiere din fișier).
    *   Clasa `Activity` este `abstract`, forțând dezvoltatorul să folosească implementări concrete specifice (Apel, Email), deoarece o "activitate" generică nu are sens în contextul de business.

---

## 2. Principiile SOLID

### SRP - Single Responsibility Principle (Principiul Responsabilității Unice)
O clasă trebuie să aibă un singur motiv să se schimbe.
*   **Lead.cs (Domain)**: Responsabilă strict pentru structura datelor unui Lead. Nu conține logică de salvare sau trimitere emailuri.
*   **LeadService.cs (Application)**: Responsabil strict pentru logica de business (ex: validări, conversia Lead -> Client). Nu interoghează direct baza de date, ci deleagă această responsabilitate.
*   **InMemoryRepository.cs (Infrastructure)**: Responsabil strict pentru mecanismul de stocare a datelor.

### OCP - Open/Closed Principle (Principiul Deschis/Închis)
Entitățile software trebuie să fie deschise pentru extindere, dar închise pentru modificare.
*   **Extindere Entități**: Putem adăuga proprietăți în `BaseEntity` (ex: `ModifiedBy`) și toate entitățile vor primi această funcționalitate fără a le modifica direct.
*   **Extindere Activități**: Putem adăuga `MeetingActivity` creând o clasă nouă care moștenește `Activity`, fără a modifica clasa de bază sau codul existent care consumă `Activity` (atâta timp cât folosește abstracția).
*   **Extindere Stocare**: Putem crea `SqlRepository` implementând `IRepository`, fără a modifica nicio linie de cod din `LeadService`.

### LSP - Liskov Substitution Principle (Principiul Substituției Liskov)
Obiectele unei superclase trebuie să poată fi înlocuite cu obiecte ale subclaselor fără a afecta corectitudinea programului.
*   **Activity**: Oriunde codul așteaptă un obiect de tip `Activity`, putem furniza un `CallActivity` fără a "strica" aplicația.
*   **Repository**: `InMemoryRepository<Lead>` implementează corect contractul `IRepository<Lead>` și poate fi folosit oriunde se cere interfața. Nu aruncă excepții neașteptate (ex: NotImplementedException) pentru metodele din interfață.

### ISP - Interface Segregation Principle (Principiul Segregării Interfețelor)
Clienții nu trebuie forțați să depindă de interfețe pe care nu le folosesc.
*   **Servicii Specifice**: Am definit `ILeadService` care conține doar metodele relevante pentru Lead-uri (`ConvertLeadToClient`). Nu am creat un `ISuperZmeuService` care să facă totul.
*   **Repository Focalizat**: `IRepository` conține doar metodele CRUD standard. Dacă am avea nevoie de căutări complexe specifice doar pentru Deal-uri, am crea `IDealRepository : IRepository<Deal>` cu acele metode extra, fără a polua interfața generică.

### DIP - Dependency Inversion Principle (Principiul Inversării Dependențelor)
Modulele de nivel înalt nu trebuie să depindă de modulele de nivel jos. Ambele trebuie să depindă de abstracții.
*   **Exemplu Concret**: 
    *   **Nivel Înalt**: `LeadService` (Business Logic).
    *   **Nivel Jos**: `InMemoryRepository` (Data Access).
    *   **Greșit (Violare DIP)**: Dacă `LeadService` ar face `new InMemoryRepository()`.
    *   **Corect (Implementat)**: `LeadService` cere în constructor `IRepository<Lead>` (Interfață).
    *   **Injecția**: În `Program.cs`, containerul DI “injectează” instanța concretă de `InMemoryRepository` atunci când se cere `IRepository`. Astfel, direcția dependenței este inversată (ambele depind de interfața din Domain).
