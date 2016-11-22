using System;

namespace Bitcraft.StateMachine
{
    /// <summary>
    /// Represents a state machine state.
    /// </summary>
    public class StateToken : Token
    {
        /// <summary>
        /// Initializes the StateToken instance.
        /// </summary>
        public StateToken()
        {
        }

        /// <summary>
        /// Initializes the StateToken instance.
        /// </summary>
        /// <param name="name">Name of the state token.</param>
        public StateToken(string name)
            : base(name)
        {
        }
    }

    /// <summary>
    /// Represents a state machine action.
    /// </summary>
    public class ActionToken : Token
    {
        /// <summary>
        /// Initializes the ActionToken instance.
        /// </summary>
        public ActionToken()
        {
        }

        /// <summary>
        /// Initializes the ActionToken instance.
        /// </summary>
        /// <param name="name">Name of the action token.</param>
        public ActionToken(string name)
            : base(name)
        {
        }
    }

    /// <summary>
    /// Represents a uniquely identifiable entity.
    /// </summary>
    public abstract class Token : IEquatable<Token>
    {
        private Guid id;
        private string name;

        /// <summary>
        /// Initializes the Token instance.
        /// </summary>
        protected Token()
        {
            id = Guid.NewGuid();
        }

        /// <summary>
        /// Initializes the Token instance.
        /// </summary>
        /// <param name="name">Name of the token.</param>
        protected Token(string name)
            : this()
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            this.name = name.Trim();
            if (this.name.Length == 0)
                throw new ArgumentException($"Invalid '{nameof(name)}' argument. It must not be empty.");
        }

        /// <summary>
        /// Provides a string representation of the token.
        /// </summary>
        /// <returns>Returns the string representation of the token.</returns>
        public override string ToString()
        {
            return name ?? "(unnamed token)";
        }

        /// <summary>
        /// Checks whether the current token is the same as another one.
        /// </summary>
        /// <param name="other">The other token to check equality upon.</param>
        /// <returns>Returns true if both tokens are the same, false otherwise.</returns>
        public bool Equals(Token other)
        {
            if ((object)other == null)
                return false;

            return id == other.id;
        }

        /// <summary>
        /// Checks whether the current token is the same as another one.
        /// </summary>
        /// <param name="obj">The other token to check equality upon.</param>
        /// <returns>Returns true if both tokens are the same, false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if ((obj is Token) == false)
                return false;

            return Equals((Token)obj);
        }

        /// <summary>
        /// Provides the hash code of the token.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        /// <summary>
        /// Checks whether the tokens a and b are the same.
        /// </summary>
        /// <param name="a">Left hand-side token.</param>
        /// <param name="b">Right hand-side token.</param>
        /// <returns>Returns true if both tokens are the same, false otherwise.</returns>
        public static bool operator ==(Token a, Token b)
        {
            if (object.ReferenceEquals(a, b))
                return true;

            if ((object)a == null || (object)b == null)
                return false;

            return a.Equals(b);
        }

        /// <summary>
        /// Checks whether the tokens a and b are different.
        /// </summary>
        /// <param name="a">Left hand-side token.</param>
        /// <param name="b">Right hand-side token.</param>
        /// <returns>Returns true if both tokens are different, false otherwise.</returns>
        public static bool operator !=(Token a, Token b)
        {
            return !(a == b);
        }
    }
}
