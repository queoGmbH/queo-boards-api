using System;
using System.Linq;

namespace Queo.Boards.Core.Tests.Builders {
    /// <summary>
    /// Basisklasse für alle Hilfsklassen, zur Erzeugung von Domain-Objekten.
    /// </summary>
    /// <typeparam name="TBuild"></typeparam>
    public abstract class Builder<TBuild> : IBuilder<TBuild> {
        private static readonly Random _random = new Random(DateTime.Now.Millisecond);

        public static implicit operator TBuild(Builder<TBuild> builder) {
            return builder.Build();
        }

        public abstract TBuild Build();

        /// <summary>
        ///     Gibt eine zufällige Zahl mit einer bestimmten Anzahl an Stellen zurück
        /// </summary>
        /// <param name="length">Die Länge des zufälligen Strings</param>
        /// <returns></returns>
        protected long GetRandomNumber(int length) {
            string chars = @"0123456789";
            string randomNumberAsString = new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
            return long.Parse(randomNumberAsString);
        }

        /// <summary>
        ///     Gibt einen zufälligen String zurück
        /// </summary>
        /// <param name="length">Die Länge des zufälligen Strings</param>
        /// <returns></returns>
        protected string GetRandomString(int length) {
            string chars = @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}