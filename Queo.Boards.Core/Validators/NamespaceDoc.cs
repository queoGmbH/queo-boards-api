namespace Queo.Boards.Core.Validators {
    /// <summary>
    ///     Die Klassen in diesem Namespace dienen der Validierung von Daten.
    ///     Dazu zählen u.a. benötigte Eigenschaften, die an Klassen gesetzt werden müssen oder Wertebereiche in denen sich
    ///     Eigenschaften bewegen müssen.
    ///     Die Validatoren dienen nur der Validierung von Nutzereingaben, nicht jedoch der Validierung von Business-Logik. So
    ///     kann zum Beispiel der Name für ein Board gegen die Bedingung validiert werden, dass dieser nicht länger als 100
    ///     Zeichen ist. Jedoch sollte nicht validiert werden, ob beim Ändern eines Boards dieses tatsächlich geändert werden
    ///     kann, was zum Beispiel bei archivierten Boards nicht möglich ist.
    /// </summary>
    public class NamespaceDoc {
    }
}