/// <summary>
/// Represents a uniquely identifiable entity.
/// </summary>
export class Token {
    private static _globalId: number = 1;

    private _id: number;
    private _name: string;

    /// <summary>
    /// Initializes the Token instance.
    /// </summary>
    constructor(displayName: string) {
        this._id = Token._globalId++;
        this._name = name;
    }

    getIdentifier(): number {
        return this._id;
    }

    /// <summary>
    /// Provides a string representation of the token.
    /// </summary>
    /// <returns>Returns the string representation of the token.</returns>
    toString(): string {
        return (!this._name) ? "(unnamed token)" : this._name;
    }

    /// <summary>
    /// Checks whether the current token is the same as another one.
    /// </summary>
    /// <param name="other">The other token to check equality upon.</param>
    /// <returns>Returns true if both tokens are the same, false otherwise.</returns>
    equals(other: Token): boolean {
        return this._id === other._id;
    }
}

export class StateToken extends Token {
}

export class ActionToken extends Token {
}
