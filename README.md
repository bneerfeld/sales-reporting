# SalesReporting

Das hier ist meine Lösung für die Refactoring-Aufgabe im Rahmen meiner Bewerbung bei ADS Steuer. Vorgegeben war ein kleines Stück Code, das einen monatlichen Verkaufsbericht erstellt. Meine Aufgabe: den Code überarbeiten und zeigen, wie ich an so etwas herangehe.

## Was mir als Erstes aufgefallen ist

Der Code war so nicht kompilierbar. An mehreren Stellen fehlten schließende Klammern und Semikolons, und teilweise wurde auf falsche Variablen verwiesen. Dazu kommt `double total`, das nie initialisiert wird. Damit lässt der Compiler die erste Rechnung mit `total` gar nicht erst durch.

## Die Schwächen

Alles steckt in einer einzigen Methode: Datei einlesen, filtern, umrechnen, Bericht bauen, Datei schreiben. Will man nur die Umrechnung testen, braucht man dafür eine echte `sales.json` auf der Platte. Und jede kleine Änderung, egal ob am Format oder an einem Kurs, landet in derselben Methode. Richtige Checks gibt es auch keine. Ein Monat 13 läuft einfach durch und ergibt einen Bericht mit 0 Umsatz. Fehlt die Datei, fliegt eine `FileNotFoundException`, und niemand fängt sie ab.

Für Geld wird `double` verwendet. Das sollte man in C# nicht machen, dafür gibt es `decimal`. Mit `double` kommt bei `0.1 * 3` eben nicht exakt `0.3` raus, und bei Beträgen fällt so etwas irgendwann auf.

Die Währungen werden über eine Kette von `if`-Abfragen mit festen Strings und Kursen umgerechnet. Das ist schwer zu lesen und nicht sauber. Kommt eine Währung dazu, wird die Kette länger, und eine unbekannte Währung fällt einfach still raus. Ich achte allgemein darauf, dass ein Fehler entweder laut knallt oder zumindest geloggt wird. Hier soll es knallen, denn sonst stimmt am Ende die Summe nicht und keiner merkt es.

## Mein Ansatz

Ich wollte zeigen, dass ich mit modernen Mitteln arbeiten kann, ohne zu übertreiben. Ehrlich gesagt fiel es mir schwer, die Grenze zu ziehen: Was ist wirklich die richtige Lösung, und was wird erwartet? Meine Strategie war deshalb folgende: Ich zeige, was ich kann. Besonders wichtig war mir, zu zeigen, wie man strukturiert und mit Abstraktion entwickelt. Am wichtigsten fand ich aber, dass sich Software testen lässt.

Während der Bearbeitung kam mir aber immer wieder ein Punkt aus der Stellenanzeige in den Kopf, nämlich die Prinzipien KISS, DRY und YAGNI. Man kann davon halten, was man möchte, aber ich denke, diese drei Prinzipien sind ein Grundstein der Softwareentwicklung. Wie man sie versteht und anwendet, ist allerdings eine Diskussion für sich. Ich habe versucht, den Rahmen nicht zu sprengen. Ich wollte es aber nicht einfach simpel halten oder nur das Nötigste tun, sondern auch zeigen, wer ich bin: Softwareentwickler durch und durch. An manchen Stellen habe ich Abstriche gemacht, die ich später noch genauer beschreibe und hervorhebe.

Für das Refactoring habe ich mich für einen kleinen Dienst auf Basis des .NET Generic Host entschieden, weil das für mich inzwischen der Standard ist. Dependency Injection und Logging sind so direkt dabei, und Jahr und Monat werden als Parameter über die Kommandozeile übergeben. Ich halte das für die sauberste Lösung.

Bei den Smart Enums war es ähnlich. Als ich die if-Ketten sah, gab es für mich zwei Wege: ein switch oder ein Enum. Da normale Enums keine Logik tragen können, habe ich Smart Enums gewählt. Mit ihnen arbeite ich privat und habe sie auch schon im Job eingesetzt. Jede Währung kennt jetzt ihren eigenen Kurs, und die if-Ketten sind vollständig entfernt.

Um meinem eigenen Anspruch von Anfang an gerecht zu werden, habe ich gleich zu Beginn ein Testprojekt angelegt und verwende dafür XUnit. Die Testfälle, die ich im Kopf hatte, waren folgende: FOLGENDEN ...

Ich weiß, was Tests sind und wie man sie einsetzt, habe praktisch aber noch relativ wenig Erfahrung damit. Bei Zahlen, Berechnungen und klarer Logik sind Tests trotzdem immer ein Gewinn, und das trifft hier eindeutig zu.

## Abweichungen und Abstriche

### Bewusste Abweichungen vom Original

Streng genommen ändert ein Refactoring das Verhalten nicht. An zwei Stellen bin ich davon bewusst abgewichen:

- Der Bericht wird nicht mehr als Textdatei `report_{year}_{month}.txt` gespeichert, sondern als JSON-Datei pro Report. Eine Textdatei mit Überschrift und Trennlinie ist nur für Menschen lesbar. Praktisch gesehen wird das aber kaum der Anwendungsfall sein. Sicher wird es irgendwann eine UI geben, die so etwas anzeigt. Nüchtern betrachtet war für mich klar, das JSON-Format zu verwenden: Es lässt sich direkt weiterverarbeiten und bleibt trotzdem für Menschen lesbar.

- Da es keine konkreten Rahmenbedingungen gab, habe ich statt einer festen `sales.json` als Eingabequelle die Möglichkeit implementiert, mehrere Reports aus `Data/` zu verarbeiten. In der Realität ist das sicher sinnvoll. Zum Ablauf ist Folgendes zu sagen: Ein fehlerhafter Report unterbricht nicht den gesamten Lauf, sondern wird übersprungen und wird im sowohl über den Logger als auch im Exit-Code gemeldet.

### Was ich in der Realität bei diesem Projekt anders machen würde
- **Ablauf:** Der `ReportJob` läuft einmal und wird direkt aus `Program.cs` gestartet. Wenn die Berichte regelmäßig erstellt werden sollen, etwa an jedem Monatsersten, würde ich daraus einen `BackgroundService` mit Timer machen.
- **Ordnerstruktur:** Derzeit ist der Code nach technischen Schichten sortiert (`Domain`, `Persistence`, `Reporting`). Bei dieser Größe genügt das. Mit jedem weiteren Interface und Writer würde ich aber nach Features schneiden, damit alles, was zu einer Funktion gehört, an einem Ort liegt.
- **Validierung:** Die Eingaben werden schon bei der Registrierung mit `.Validate(...)` geprüft. Für ein paar zusätzliche Regeln reichen eigene `IValidateOptions`-Klassen. Sobald die Regeln komplexer werden, weil sie zum Beispiel voneinander abhängen oder an mehreren Stellen gebraucht werden, würde ich eine Bibliothek wie FluentValidation einsetzen. Sie lässt sich über einen `IValidateOptions`-Adapter ebenfalls an die Options anbinden.

**Mögliche Erweiterungen**
- **Dateitypen:** Ursprünglich wollte ich auch andere Dateitypen unterstützen, vor allem CSV. Diesen Gedanken habe ich aber bewusst verworfen, weil er nicht zum Refactoring gepasst hätte.
- **Parameterübergabe:** Ich hatte außerdem überlegt, die von mir eingeführte Parameterübergabe des Dienstes zu erweitern. Einzelne Parameter könnten dann wegfallen, sodass man zum Beispiel für ein ganzes Jahr die Reports aller Monate bekommt oder einfach alle Reports. Auch das hätte aber nicht wirklich zum Refactoring gepasst.

## Projekt starten

Voraussetzung ist das .NET 10 SDK. Jahr und Monat werden beim Start als Parameter übergeben. Ohne `--Report` laufen alle Reports aus `Data/`, mit `--Report <Name>` (ohne `.json`) nur dieser eine:

```bash
dotnet run --project SalesReporting -- --Year 2026 --Month 10
dotnet run --project SalesReporting -- --Year 2026 --Month 10 --Report berlin
```

Jeder Bericht landet als JSON-Datei im Ordner `Reports/` neben der Anwendung, z. B. `Reports/berlin_2026_10.json`. Der Ordner lässt sich über `Folders:Reports` in der `appsettings.json` ändern.

Mit Rider oder auch Visual Studio (nicht getestet) geht es auch ohne Kommandozeile. Die Startprofile `All`, `Valid` und `Invalid` übergeben die Parameter schon, man muss also nur auf Run oder Debug drücken.

Sobald Tests dazukommen, laufen sie über:

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
│   ├── Program.cs                 Einstiegspunkt, baut den Host und startet den ReportJob
│   ├── DependencyInjection.cs     Registrierung der Dienste
│   ├── ExitCode.cs                Exit-Codes der Anwendung
│   ├── Refactoring.cs             Ausgangscode der Vorgabe (auskommentiert)
│   ├── Domain/
│   │   ├── Currency.cs            Smart Enum mit Umrechnungskurs
│   │   ├── Sale.cs
│   │   └── MonthlySalesReport.cs
│   ├── Persistence/
│   │   ├── FolderOptions.cs       Ordner für Verkaufsdaten und Berichte
│   │   ├── IReportWriter.cs
│   │   ├── ISaleRepository.cs
│   │   ├── JsonReportWriter.cs    schreibt den Bericht als JSON-Datei
│   │   ├── JsonSaleRepository.cs
│   │   ├── SaleJsonParser.cs      liest Verkäufe aus einem Stream
│   │   └── SalesFileLocator.cs    findet die Datendatei zum Report-Namen
│   ├── Reporting/
│   │   ├── ReportJob.cs           steuert den Ablauf über alle Reports
│   │   ├── ReportRequestOptions.cs Jahr, Monat und Report aus der Kommandozeile
│   │   └── ReportService.cs       filtert und summiert die Verkäufe
│   ├── Data/
│   │   ├── berlin.json, …         fünf Filialen mit Verkäufen von Juni bis Oktober 2026,
│   │   │                          hamburg und koeln enthalten fehlerhafte Einträge
│   └── appsettings*.json          Konfiguration je Umgebung
└── SalesReporting.Tests/
```
