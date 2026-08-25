Dostępne typy:

Litera	Typ zmiany	Kiedy użyć
m	MAJOR	Duża zmiana (nowa funkcja, zmiana interfejsu)
n	MINOR	Mała zmiana (poprawka, optymalizacja)
p	PATCH	Bardzo mała zmiana (hotfix, literówka)
f	FIX	Poprawka błędu
a	ALPHA	 Mała poprawka
b	BETA	Wersja testowa (stabilniejsza)

powershell -ExecutionPolicy Bypass -File .\UpdateVersion.ps1 m   # MAJOR

```markdown
# OpenTTD Manager

[![.NET](https://img.shields.io/badge/.NET-10.0-blueviolet)](https://dotnet.microsoft.com/)
[![Windows Forms](https://img.shields.io/badge/Windows_Forms-10.0-blue)](https://learn.microsoft.com/pl-pl/dotnet/desktop/winforms/)
[![SQLite](https://img.shields.io/badge/SQLite-3.0-lightgrey)](https://www.sqlite.org/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Version](https://img.shields.io/badge/Version-2.0.0-orange)](https://github.com/Marcys06/ConsoleApp1)

Aplikacja desktopowa napisana w **C# .NET 10** z wykorzystaniem **Entity Framework Core** i **SQLite**, służąca do zarządzania rozkładami jazdy, pociągami, stacjami i trasami dla gry **OpenTTD** (_Open Transport Tycoon Deluxe_).

![OpenTTD Manager](https://img.shields.io/badge/Status-Active-success)

---

## 📌 Cel projektu

OpenTTD Manager to narzędzie, które pozwala graczom OpenTTD na wygodne planowanie rozkładów jazdy poza grą. Aplikacja umożliwia:

- ✅ Tworzenie i edycję pociągów z parametrami technicznymi (Vmax, moc, masa)
- ✅ Definiowanie stacji z lokalizacją (współrzędne geograficzne lub mapowe)
- ✅ Tworzenie tras w układzie **A → B → A** (pętla powrotna)
- ✅ Dodawanie wielu kursów (odjazdów) na jednej trasie o różnych godzinach
- ✅ Automatyczne obliczanie czasów przyjazdów na podstawie Vmax pociągu
- ✅ Przechowywanie danych w lokalnej bazie SQLite

---

## ✨ Główne funkcjonalności

### 🚂 Zarządzanie pociągami (`Trains`)
- **Dodawanie, edycja, usuwanie** pociągów
- Parametry: nazwa, model, Vmax (km/h), moc (kW), masa (t), rok produkcji, typ napędu, ścieżka do ikony
- Wyszukiwanie i filtrowanie

### 🏢 Zarządzanie stacjami (`Stations`)
- **Dodawanie, edycja, usuwanie** stacji
- Parametry: nazwa, współrzędne geograficzne, typ obsługi (pasażerska/towarowa)

### 🛤️ Zarządzanie trasami (`Routes`)
- Tworzenie tras w układzie **Stacja A → Stacja B → Stacja A**
- Definiowanie kolejności przystanków i czasu postoju
- Możliwość dezaktywacji trasy

### 🕐 Zarządzanie rozkładami (`Schedules`)
- Dodawanie wielu kursów na jednej trasie
- Określanie godziny odjazdu i przypisywanie pociągu
- Automatyczne obliczanie czasów przyjazdów

### ⏱️ Obliczanie czasów przejazdu (`ScheduleTravelTimes`)
- Każdy kurs ma własne czasy przejazdu
- Obliczanie na podstawie Vmax i odległości
- Możliwość ręcznej edycji

---

## 🗄️ Struktura bazy danych

```
Trains ────────┐
               │
Stations ──────┼─── RouteStations ─── Routes
               │         │
               │         ▼
               └─── Schedules ─── ScheduleTravelTimes
```

**Kluczowe tabele:**
- `Trains` – pociągi
- `Stations` – stacje
- `Routes` – trasy (A → B → A)
- `RouteStations` – przystanki na trasie
- `Schedules` – kursy/rozkłady
- `ScheduleTravelTimes` – czasy przejazdu

---

## 🛠️ Technologie

| Komponent | Technologia |
|-----------|-------------|
| **Język** | C# (.NET 10.0) |
| **Framework** | .NET 10.0 LTS |
| **ORM** | Entity Framework Core 10.0.11 |
| **Baza danych** | SQLite (lokalna) |
| **Interfejs użytkownika** | Windows Forms |
| **Kontrola wersji** | Git + GitHub |
| **Migracje** | EF Core Migrations |

---

## 📁 Struktura projektu

```
OpenTTDManager/
├── TTD.Data/                   # Warstwa danych (Class Library)
│   ├── Models/                 # Modele encji
│   │   ├── Train.cs
│   │   ├── Station.cs
│   │   ├── Route.cs
│   │   ├── RouteStation.cs
│   │   ├── Schedule.cs
│   │   └── ScheduleTravelTime.cs
│   ├── AppDbContext.cs         # Kontekst EF Core
│   └── Migrations/             # Migracje bazy danych
│
├── TTD.Core/                   # Warstwa logiki biznesowej
│   ├── Interfaces/             # Interfejsy serwisów
│   │   ├── ITrainService.cs
│   │   ├── IStationService.cs
│   │   ├── IRouteService.cs
│   │   └── IScheduleService.cs
│   └── Services/               # Implementacje serwisów
│       ├── TrainService.cs
│       ├── StationService.cs
│       ├── RouteService.cs
│       └── ScheduleService.cs
│
├── TTD.Main/                   # Główny projekt (launcher + UI)
│   ├── Program.cs              # Launcher z menu
│   ├── UI/Forms/               # Formularze Windows Forms
│   │   ├── MainForm.cs
│   │   ├── TrainForm.cs
│   │   ├── StationForm.cs
│   │   ├── RouteForm.cs
│   │   └── ScheduleForm.cs
│   ├── API/                    # Serwer REST API
│   ├── ConsoleTools/           # Narzędzia konsolowe (seed, eksport, import)
│   ├── Reports/                # Generowanie raportów
│   └── Database/               # Zarządzanie bazą danych
│
├── .gitignore
├── CHANGELOG.md                # Historia zmian
└── README.md
```

---

## 📊 Przykładowe dane

### 🚂 Pociągi

| Nazwa | Model | Vmax | Moc | Masa | Elektryczny |
|-------|-------|------|-----|------|-------------|
| EU07-101 | EU07 | 160 | 2400 | 120 | ✅ Tak |
| EN57-001 | EN57 | 120 | 1800 | 100 | ✅ Tak |
| Pendolino-01 | Pendolino | 250 | 5000 | 180 | ✅ Tak |

### 🏢 Stacje

| Nazwa | Szerokość | Długość | Pasażerska | Towarowa |
|-------|-----------|---------|------------|----------|
| Warszawa Centralna | 52.2297 | 21.0122 | ✅ | ❌ |
| Kraków Główny | 50.0614 | 19.9386 | ✅ | ❌ |
| Gdańsk Główny | 54.3520 | 18.6466 | ✅ | ✅ |

### 🛤️ Trasa: Warszawa ↔ Kraków

| Kolejność | Stacja | Postój (min) |
|-----------|--------|--------------|
| 1 | Warszawa Centralna | 5 |
| 2 | Kraków Główny | 10 |
| 3 | Warszawa Centralna | 10 |

### 🕐 Kursy (odjazdy z Warszawy)

| Godzina | Pociąg | Uwagi |
|---------|--------|-------|
| 03:00 | EU07-101 | nocny |
| 06:00 | EN57-001 | poranny |
| 12:00 | Pendolino-01 | południowy |
| 19:00 | EU07-101 | wieczorny |

---

## 🚀 Instalacja i uruchomienie

### Wymagania
- [.NET 10.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Git](https://git-scm.com/)

### Szybki start

1. **Sklonuj repozytorium**
   ```bash
   git clone https://github.com/Marcys06/ConsoleApp1.git
   cd ConsoleApp1
   ```

2. **Przejdź do projektu i zbuduj**
   ```bash
   cd TTD.Main
   dotnet build
   ```

3. **Uruchom aplikację**
   ```bash
   dotnet run
   ```

### Użycie przez Visual Studio
1. Otwórz plik `ConsoleApp1.sln`
2. Ustaw `TTD.Main` jako projekt startowy
3. Naciśnij **F5**

---

## 🎯 Planowane rozszerzenia

| Funkcjonalność | Opis |
|----------------|------|
| **Import/Export** | Eksport i import danych do JSON/CSV |
| **Mapa** | Wizualizacja tras na OpenStreetMap |
| **Raporty PDF** | Generowanie raportów w formacie PDF |
| **Opóźnienia** | Rejestrowanie opóźnień pociągów |
| **Dni tygodnia** | Określanie dni kursowania |
| **Użytkownicy** | System logowania dla wielu użytkowników |
| **API REST** | Integracja z OpenTTD przez HTTP |

---

## 🔄 System wersjonowania

Projekt używa systemu **SemVer** z datą:
```
Format: YYYY.MM.DD-HHMM-MAJOR.MINOR.PATCH[-sufiks]
Przykład: 2026.08.25-2210-1.0.0
```

**Typy zmian:**
- `m` = MAJOR (duża zmiana, nowa funkcja)
- `n` = MINOR (mała zmiana, poprawka)
- `p` = PATCH (bardzo mała zmiana)
- `f` = FIX (poprawka błędu)
- `a` = ALPHA (wersja testowa)
- `b` = BETA (wersja testowa)

---

## 🧑‍💻 Dla kogo jest ten projekt?

| Grupa | Zastosowanie |
|-------|--------------|
| **Gracze OpenTTD** | Planowanie rozkładów poza grą |
| **Studenci** | Nauka C#, EF Core, SQLite, Git |
| **Programiści** | Wzorzec architektury aplikacji |

---

## 👤 Autor

**Autor:** Marcos06  
**Data:** 2026  
**Licencja:** MIT

---

## 🔗 Linki

- [OpenTTD](https://www.openttd.org)
- [.NET 10](https://dotnet.microsoft.com)
- [GitHub](https://github.com/Marcys06/ConsoleApp1)

---

## 📋 Podsumowanie

**OpenTTD Manager** to kompletne narzędzie do zarządzania logistyką kolejową w grze OpenTTD. Aplikacja łączy prostotę obsługi z zaawansowanymi możliwościami planowania rozkładów jazdy. Dzięki przejrzystej architekturze i nowoczesnym technologiom (.NET 10, EF Core, SQLite) jest łatwa w rozwoju i utrzymaniu.

---

