# CHANGELOG

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




