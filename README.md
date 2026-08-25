
# OpenTTD Manager

## 📌 1. Cel projektu

**OpenTTD Manager** to aplikacja desktopowa napisana w **C# .NET** z wykorzystaniem **Entity Framework Core** i **SQLite**, służąca do zarządzania rozkładami jazdy, pociągami, stacjami i trasami dla gry **OpenTTD** (*Open Transport Tycoon Deluxe*).

Aplikacja umożliwia:
- Tworzenie i edycję pociągów z ich parametrami technicznymi (Vmax, moc, masa)
- Definiowanie stacji z lokalizacją (współrzędne geograficzne lub mapowe)
- Tworzenie tras w układzie **A → B → A** (pętla powrotna)
- Dodawanie wielu kursów (odjazdów) na jednej trasie o różnych godzinach
- Automatyczne obliczanie czasów przyjazdów i odjazdów na podstawie Vmax pociągu
- Przechowywanie danych w lokalnej bazie SQLite

---

## 🎯 2. Główne funkcjonalności

### 2.1. Zarządzanie pociągami (`Trains`)
- Dodawanie, edycja, usuwanie pociągów
- Parametry:
  - Nazwa i model
  - Prędkość maksymalna (Vmax) w km/h
  - Moc (kW) i masa (tony)
  - Rok produkcji
  - Typ napędu (elektryczny/spalinowy)
  - Ścieżka do ikony (opcjonalnie)

### 2.2. Zarządzanie stacjami (`Stations`)
- Dodawanie, edycja, usuwanie stacji
- Parametry:
  - Nazwa stacji
  - Współrzędne geograficzne (szerokość/długość)
  - Współrzędne mapy OpenTTD (X/Y) – opcjonalnie
  - Typ obsługi (pasażerska/towarowa)

### 2.3. Zarządzanie trasami (`Routes`)
- Tworzenie tras w układzie **Stacja A → Stacja B → Stacja A**
- Definiowanie kolejności przystanków
- Określanie czasu postoju na każdej stacji
- Możliwość dezaktywacji trasy

### 2.4. Zarządzanie rozkładami/kursami (`Schedules`)
- Dodawanie wielu kursów na jednej trasie
- Określanie godziny odjazdu z pierwszej stacji
- Przypisywanie konkretnego pociągu do kursu
- Określanie okresu ważności kursu (od–do)
- Automatyczne obliczanie czasów przyjazdów na każdą stację

### 2.5. Obliczanie czasów przejazdu (`ScheduleTravelTimes`)
- Każdy kurs ma własne czasy przejazdu między stacjami
- Czasy są obliczane na podstawie Vmax pociągu i odległości
- Możliwość ręcznej edycji czasów

---

## 🗄️ 3. Struktura bazy danych

### Diagram relacji:

```
┌─────────────┐       ┌──────────────────┐       ┌─────────────┐
│   Trains    │       │    Routes        │       │  Stations   │
├─────────────┤       ├──────────────────┤       ├─────────────┤
│ Id          │◄──────│ Id               │       │ Id          │
│ Name        │       │ Name             │       │ Name        │
│ Model       │       │ IsActive         │       │ Latitude    │
│ VMax        │       │ Notes            │       │ Longitude   │
│ Power       │       └──────────────────┘       │ MapX        │
│ Weight      │              │                    │ MapY        │
│ ModelYear   │              │                    │ IsPassenger │
│ IsElectric  │              │                    │ IsCargo     │
│ ImagePath   │              │                    └─────────────┘
└─────────────┘              │                           ▲
       │                     │                           │
       │                     ▼                           │
       │           ┌──────────────────┐                  │
       │           │  RouteStations   │                  │
       │           ├──────────────────┤                  │
       │           │ RouteId (FK)     │──────────────────┘
       │           │ StationId (FK)   │──────────────────┘
       │           │ StopOrder        │
       │           │ StopDuration     │
       │           │ DistanceFromPrev │
       │           └──────────────────┘
       │                     │
       │                     ▼
       │           ┌──────────────────┐
       └──────────▶│   Schedules      │
                   ├──────────────────┤
                   │ Id               │
                   │ RouteId (FK)     │
                   │ TrainId (FK)     │◄──────────────────────┐
                   │ DepartureTime    │                       │
                   │ IsActive         │                       │
                   │ ValidFrom        │                       │
                   │ ValidTo          │                       │
                   │ Notes            │                       │
                   └──────────────────┘                       │
                            │                                  │
                            ▼                                  │
                   ┌──────────────────┐                       │
                   │ ScheduleTravel   │                       │
                   │     Times        │                       │
                   ├──────────────────┤                       │
                   │ Id               │                       │
                   │ ScheduleId (FK)  │───────────────────────┘
                   │ RouteStationId   │───────────────────────┐
                   │ TravelTimeMinutes│                       │
                   └──────────────────┘                       │
                            │                                  │
                            └──────────────────────────────────┘
```

---

## 🛠️ 4. Technologie

| Komponent | Technologia |
|-----------|-------------|
| **Język** | C# (.NET 10.0) |
| **Framework** | .NET 10.0 LTS |
| **ORM** | Entity Framework Core 10.0.11 |
| **Baza danych** | SQLite (lokalna) |
| **Interfejs użytkownika** | Windows Forms / WPF (do wyboru) |
| **Kontrola wersji** | Git + GitHub |
| **Migracje** | EF Core Migrations |

---

## 📁 5. Struktura projektu

```
OpenTTDManager/
├── TTD.sln                           # Rozwiązanie Visual Studio
│
├── TTD.Data/                         # Warstwa danych (Class Library)
│   ├── Models/
│   │   ├── Train.cs                  # Model pociągu
│   │   ├── Station.cs                # Model stacji
│   │   ├── Route.cs                  # Model trasy
│   │   ├── RouteStation.cs           # Pośrednia trasa-stacja
│   │   ├── Schedule.cs               # Model kursu/rozkładu
│   │   ├── ScheduleTravelTime.cs     # Czasy przejazdu dla kursu
│   │   └── DTOs/
│   │       └── ScheduleDetailsDto.cs # DTO do wyświetlania rozkładów
│   ├── AppDbContext.cs               # Kontekst EF Core
│   └── Migrations/                   # Migracje bazy danych (generowane)
│
├── TTD.UI/                           # Warstwa prezentacji (WinForms/WPF)
│   ├── Forms/
│   │   ├── MainForm.cs               # Główne okno aplikacji
│   │   ├── TrainForm.cs              # Zarządzanie pociągami
│   │   ├── StationForm.cs            # Zarządzanie stacjami
│   │   ├── RouteForm.cs              # Zarządzanie trasami
│   │   └── ScheduleForm.cs           # Zarządzanie rozkładami
│   ├── appsettings.json              # Konfiguracja (connection string)
│   └── Program.cs                    # Punkt startowy aplikacji
│
├── TTD.Core/                         # Warstwa logiki biznesowej (opcjonalnie)
│   ├── Services/
│   │   ├── TrainService.cs
│   │   ├── RouteService.cs
│   │   └── ScheduleService.cs
│   └── Helpers/
│       └── TimeCalculator.cs         # Kalkulator czasów przejazdu
│
├── .gitignore                        # Pliki ignorowane przez Git
└── README.md                         # Opis projektu
```

---

## 📊 6. Przykładowe dane

### Pociągi:
| Nazwa | Model | Vmax | Moc | Masa | Elektryczny |
|-------|-------|------|-----|------|-------------|
| EU07-101 | EU07 | 160 | 2400 | 120 | Tak |
| EN57-001 | EN57 | 120 | 1800 | 100 | Tak |
| Pendolino-01 | Pendolino | 250 | 5000 | 180 | Tak |

### Stacje:
| Nazwa | Szerokość | Długość | Pasażerska | Towarowa |
|-------|-----------|---------|------------|----------|
| Warszawa Centralna | 52.2297 | 21.0122 | Tak | Nie |
| Kraków Główny | 50.0614 | 19.9386 | Tak | Nie |
| Gdańsk Główny | 54.3520 | 18.6466 | Tak | Tak |

### Trasa (Warszawa → Kraków → Warszawa):
| Kolejność | Stacja | Postój (min) |
|-----------|--------|--------------|
| 1 | Warszawa Centralna | 5 |
| 2 | Kraków Główny | 10 |
| 3 | Warszawa Centralna | 10 |

### Kursy (odjazdy z Warszawy):
| Godzina | Pociąg | Uwagi |
|---------|--------|-------|
| 03:00 | EU07-101 | nocny |
| 06:00 | EN57-001 | poranny |
| 12:00 | Pendolino-01 | południowy |
| 19:00 | EU07-101 | wieczorny |

---

## 🚀 7. Planowane rozszerzenia

| Funkcjonalność | Opis |
|----------------|------|
| **Import/Export** | Eksport i import danych do JSON/CSV |
| **Mapa** | Wizualizacja tras na mapie (OpenStreetMap) |
| **Raporty** | Generowanie raportów PDF z rozkładami |
| **Opóźnienia** | Rejestrowanie opóźnień pociągów |
| **Dni tygodnia** | Określanie, w które dni kursuje dany kurs |
| **Użytkownicy** | System logowania dla wielu użytkowników |
| **API** | REST API do integracji z OpenTTD |

---

## 🎓 8. Dla kogo jest ten projekt?

| Grupa | Zastosowanie |
|-------|--------------|
| **Gracze OpenTTD** | Planowanie rozkładów poza grą |
| **Studenci** | Nauka C#, EF Core, SQLite, Git |
| **Programiści** | Wzorzec do nauki architektury aplikacji |

---

## 📝 9. Autor

- **Autor:** Marcos06
- **Data:** 2026
- **Licencja:** MIT (lub inna do wyboru)

---

## 🔗 10. Linki

- **OpenTTD** – [openttd.org](https://www.openttd.org)
- **.NET 8** – [dotnet.microsoft.com](https://dotnet.microsoft.com)
- **GitHub** – [github.com/Marcos06/OpenTTDManager](https://github.com/Marcos06/OpenTTDManager) (po utworzeniu)

---

## 📋 11. Podsumowanie

**OpenTTD Manager** to kompletne narzędzie do zarządzania logistyką kolejową w grze OpenTTD. Aplikacja łączy w sobie prostotę obsługi z zaawansowanymi możliwościami planowania rozkładów jazdy. Dzięki przejrzystej architekturze i nowoczesnym technologiom (.NET 8, EF Core, SQLite) jest łatwa w rozwoju i utrzymaniu.

---

Gotowe! Możesz to wkleić do pliku `README.md` w swoim repozytorium. 🚂
