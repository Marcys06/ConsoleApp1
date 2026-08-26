# CHANGELOG

## [2026.08.26-1230-1.2.2] - 2026-08-26

### TYP ZMIANY: MINOR

### DODANE
- **Dodawanie wielu kursów naraz** - możliwość dodania wielu kursów jednocześnie
- **Generowanie regularnych kursów** - tworzenie kursów co X minut (np. co 30 minut)
- **Dodawanie nieregularnych kursów** - ręczne wpisywanie czasów (np. 8:00, 8:30, 9:15)
- **Podgląd kursów przed zapisem** - tabela z wygenerowanymi kursami
- **Automatyczne pomijanie duplikatów** - pomija kursy, które już istnieją
- **BatchScheduleForm** - nowe okno do dodawania wielu kursów
- **Przycisk "📋 Dodaj wiele"** - w ScheduleForm

### ZMIENIONE
- ScheduleForm - rozszerzenie okna na 850x500
- ScheduleForm - dodanie przycisku "Dodaj wiele"

### POPRAWIONE
- Uszkodzony tekst przycisku "Dodaj wiele"
- Brak widoczności przycisku "Dodaj wiele"

### TECHNICZNE
- Dodanie `BatchScheduleForm.cs`
- Modyfikacja `ScheduleForm.cs`
- Dodanie metody `BtnBatchAdd_Click`

---

## [2026.08.26-1204-1.2.2-beta] - 2026-08-26

### TYP ZMIANY: BETA

### DODANE
- Struktura projektów TTD.Main, TTD.Core, TTD.Data
- Konfiguracja Windows Forms dla .NET 10
- Referencje między projektami

### ZMIENIONE
- Pliki .csproj - konfiguracja i struktura
- Usunięcie OpenTTDManager.csproj
- Uproszczenie zależności NuGet

### POPRAWIONE
- Błędy Windows Forms
- Błędy duplikatów Assembly
- Błędy XML w .csproj
- Problemy z budowaniem i artefaktami
- Wykrywanie projektu startowego

---
## [2026.08.26-1152-1.2.1] - 2026-08-26


### TYP ZMIANY: PATCH

### DODANE
- Migracja bazy danych z polami `PlatformCount`, `TrackId`, `PlatformId`
- DesignTimeDbContextFactory dla migracji EF Core
- Obsługa peronów i torów (przygotowanie)

### ZMIENIONE
- AppDbContext - dodanie konstruktora bezparametrowego i OnConfiguring
- Modele Station i RouteStation - nowe pola

### POPRAWIONE
- Błąd migracji "No database provider configured"
- Błąd "table already exists"
- Błąd "no such column: PlatformId"

---
## [2026.08.26-1129-1.2.0] - 2026-08-26

### TYP ZMIANY: MINOR

### DODANE
- Indywidualne czasy przejazdu dla każdego odcinka trasy (A→B, B→C, C→B, B→A)
- Przyciski "Ustaw czasy" i "Edytuj czasy" w ScheduleEditForm
- Możliwość powiększania okna ScheduleEditForm

### ZMIENIONE
- ScheduleEditForm - zmiana rozmiaru i możliwość skalowania
- DataGridView w ScheduleEditForm - edytowalny

### POPRAWIONE
- NullReferenceException w ScheduleDetailsForm
- Brak nazw stacji w tabeli czasów

---
## [2026.08.26-1101-1.1.0] - 2026-08-26

### TYP ZMIANY: MINOR

### DODANE
- Obsługa tras z powrotami (A-B-C-B-A)
- Przycisk "Zamknij pętlę" (🔄)
- Oznaczenia powrotów na liście stacji
- Etykieta typu trasy (pętla/powroty/jednokierunkowa)

### ZMIENIONE
- Logika dodawania stacji - pozwala na wielokrotne dodawanie
- Logika odświeżania list - wyświetlanie już dodanych stacji

### POPRAWIONE
- Błąd "Usługa stacji nie jest dostępna"
- Brak wyświetlania stacji w RouteEditForm
- Brak możliwości dodania tej samej stacji

---
## [2026.08.25-2147-1.0.1-alpha] - 2026-08-25

### TYP ZMIANY: ALPHA

### ZMIANY
- Usunięto duplikat `using TTD.Data;` w `Program.cs` 

### POPRAWIONE
- 

---
## [2026.08.25-2058-1.0.0] - 2026-08-25

### TYP ZMIANY: MAJOR

### ZMIANY
- Reset ChangeLog.md

### POPRAWIONE
- 

---






