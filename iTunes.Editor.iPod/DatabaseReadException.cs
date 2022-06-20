// This file isn't generated, but this comment is necessary to exclude it from StyleCop analysis.
// <auto-generated/>

namespace IPod {

    internal class DatabaseReadException : ApplicationException {

        public DatabaseReadException (Exception e, string msg, params object[] args) : base (String.Format (msg, args),
                                                                                             e) {
        }
        
        public DatabaseReadException (string msg, params object[] args) : base (String.Format (msg, args)) {
        }
    }
}