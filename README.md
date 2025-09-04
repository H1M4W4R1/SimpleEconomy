<div align="center">
  <h1>Simple Economy</h1>
</div>

# About

Simple Economy is a subpackage of SimpleKit intended for quick and easy implementation of a basic
and more complicated economy.

*For requirements check .asmdef*

# Creating a currency
Currency is an information object that is used to store basic currency info (such as name, symbol, etc.) and events.

To create a currency simply extend `CurrencyBase` class (it's an Auto-Created ScriptableObject) and implement
desired checks and events. For reference of all available events and checks see `CurrencyBase` class.

```csharp
public sealed class DiamondsCurrency : CurrencyBase
{
     protected internal override void OnCurrencyAddFailed(
            in CurrencyAddContext context,
            in OperationResult<long> resultAmountExpected) =>
            Debug.LogError("Can't add currency :(");    
}

```

Of course you can create abstract currencies to store common properties or logic.

# Creating a wallet
Wallet is storage of currencies. To create a wallet simply extend `CurrencyWalletBase<TCurrency>` class
and implement desired events / methods. 

Beware that overriding wallet methods may disable calls to 
currency-level events as they're proxy-passed from wallet events to currency class.

Wallets after creation can be added to GameObjects to make them store currencies.

```csharp
public sealed class DiamondsWallet : CurrencyWalletBase<DiamondsCurrency>
{
    // Override desired methods
}
```

# Adding currencies

To add currencies to a wallet use `TryAdd` method. 

```csharp
OperationResult<long> currencyLeftToAdd = wallet.TryAdd(100, ModifyWalletCurrencyFlags.None, ActionSource.External);
```

You can specify `ModifyWalletCurrencyFlags` to modify wallet behavior (for example, to prevent checking
for conditions if your money has to be added). Also you can change `ActionSource` to make action
internal and prevent it from triggering events.

If you set currency to ignore checks then it will try to add currency, however you can modify add behaviour
by overriding `Add` method to enforce currency limits. In such case you should return OperationResult with amount
of currency that was not added which then will be proxied outwards.

# Taking currency

Similarly to `TryAdd` there's `TryTake` which uses same set of parameters, but instead of adding currency
it takes from wallet. Minimum balance is always enforced to be zero, so even if you ignore checks it will never allow
negative balance.

```csharp
OperationResult<long> currencyLeftToTake = wallet.TryTake(100, ModifyWalletCurrencyFlags.IgnoreConditions, ActionSource.
Internal);
```