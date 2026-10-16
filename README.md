# SalesReporting

Das hier ist meine Lösung für die Refactoring-Aufgabe im Rahmen meiner Bewerbung bei ADS Steuer. Vorgegeben war ein kleines Stück Code, das einen monatlichen Verkaufsbericht erstellt. Meine Aufgabe: den Code überarbeiten und zeigen, wie ich an so etwas herangehe.

## Was mir als Erstes aufgefallen ist

Der Code war so nicht kompilierbar. An mehreren Stellen fehlten schließende Klammern und Semikolons, und teilweise wurde auf falsche Variablen verwiesen. Dazu kommt `double total`, das nie initialisiert wird. Damit lässt der Compiler die erste Rechnung mit `total` gar nicht erst durch.

## Die Schwächen

Alles steckt in einer einzigen Methode: Datei einlesen, filtern, umrechnen, Bericht bauen, Datei schreiben. Will man nur die Umrechnung testen, braucht man dafür eine echte `sales.json` auf der Platte. Und jede kleine Änderung, egal ob am Format oder an einem Kurs, landet in derselben Methode. Richtige Checks gibt es auch keine. Ein Monat 13 läuft einfach durch und ergibt einen Bericht mit 0 Umsatz. Fehlt die Datei, fliegt eine `FileNotFoundException`, und niemand fängt sie ab.

Für Geld wird `double` verwendet. Das sollte man in C# nicht machen, dafür gibt es `decimal`. Mit `double` kommt bei `0.1 * 3` eben nicht exakt `0.3` raus, und bei Beträgen fällt so etwas irgendwann auf.

Die Währungen werden über eine Kette von `if`-Abfragen mit festen Strings und Kursen umgerechnet. Das ist schwer zu lesen und nicht sauber. Kommt eine Währung dazu, wird die Kette länger, und eine unbekannte Währung fällt einfach still raus. Ich achte allgemein darauf, dass ein Fehler entweder laut knallt oder zumindest geloggt wird. Hier soll es knallen, denn sonst stimmt am Ende die Summe nicht und keiner merkt es.

## Mein Ansatz

Ich wollte zeigen, dass ich mit modernen Mitteln arbeiten kann, ohne es zu übertreiben. Ehrlich gesagt habe ich mich am Anfang etwas schwergetan, die Linie zu ziehen, was hier wirklich gefordert ist. Für mich habe ich dann entschieden: Ich zeige, dass das Wissen da ist, auch in der Breite, und vor allem, dass ich weiß, wie Abstraktion funktioniert und wie man eine Struktur baut, die sich testen lässt.

Auf die Spitze getrieben habe ich das bewusst nicht. In der Stellenanzeige standen KISS, DRY und YAGNI als Prinzipien, die man kennen sollte. Das habe ich als kleinen Hinweis und auch als Ansporn genommen, hier zu zeigen, dass ich sie auch anwende. Ausgerechnet in der Aufgabe dagegen zu verstoßen, wäre ja ziemlich ironisch gewesen. Meine Idee war, den Code so zu nehmen, wie er ist, und ihn schlüssiger, moderner und sauberer zu schreiben. Wer reinschaut, soll sehen, dass ich weiß, wie man so etwas skaliert, ohne dass es overengineert wirkt.

Ich habe mich für einen kleinen Dienst über den .NET Generic Host entschieden, weil das für mich mittlerweile einfach der Standard ist. So habe ich Dependency Injection und Logging direkt dabei, und Jahr und Monat kommen einfach als Parameter über die Kommandozeile rein. Für mich ist das die sauberste Lösung.

Bei den Smart Enums war es ähnlich. Als ich die `if`-Ketten gesehen habe, gab es für mich zwei Wege: ein `switch` oder ein Enum. Normale Enums können aber keine Logik tragen, also habe ich zu Smart Enums gegriffen. Mit denen arbeite ich privat und habe sie auch schon im Job eingesetzt. Jede Währung kennt jetzt ihren eigenen Kurs, und die `if`-Ketten sind komplett weg. In diesem Fall halte ich das für die beste und sauberste Lösung.

Damit man auch sieht, dass der Code wirklich testbar ist, habe ich ein kleines Testprojekt dazu gebaut. Ich will dabei offen sein: Ich weiß, was Tests sind und wie man sie einsetzt, praktisch habe ich damit aber noch relativ wenig Erfahrung. Wo es um Zahlen, Berechnungen und klare Logik geht, sind Tests trotzdem immer ein Gewinn, und das ist hier definitiv der Fall.

## Projekt starten

Voraussetzung ist das .NET 10 SDK. Jahr und Monat werden beim Start als Parameter übergeben:

```bash
dotnet run --project SalesReporting -- --Year 2024 --Month 3
```

In Rider geht es auch ohne Kommandozeile. Das Startprofil `SalesReporting (Development)` übergibt die Parameter schon, man muss also nur auf Run oder Debug drücken.

Die Tests laufen über:

```bash
dotnet test
```

## Aufbau

```
SalesReporting/
├── SalesReporting.slnx
├── Directory.Build.props          gemeinsame Projekteinstellungen
├── Directory.Packages.props       zentrale Paketversionen
├── SalesReporting/
│   ├── Program.cs                 Einstiegspunkt, startet den Host
│   ├── DependencyInjection.cs     Registrierung der Dienste
│   ├── Refactoring.cs             Ausgangscode der Vorgabe (auskommentiert)
│   ├── Domain/
│   │   ├── Currency.cs            Smart Enum mit Umrechnungskurs
│   │   ├── Sale.cs
│   │   └── MonthlySalesReport.cs
│   ├── Persistence/
│   │   ├── ISaleRepository.cs
│   │   ├── JsonSaleRepository.cs
│   │   └── SalesFileOptions.cs
│   ├── Reporting/
│   │   ├── ReportService.cs       filtert und summiert die Verkäufe
│   │   └── ReportPrinter.cs       gibt den Bericht über das Logging aus
│   ├── Data/
│   │   └── sales.json             Beispieldaten
│   └── appsettings*.json          Konfiguration je Umgebung
└── SalesReporting.Tests/
```
