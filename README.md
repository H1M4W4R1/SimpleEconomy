<div align="center">
  <h1>Simple Economy</h1>
</div>

# About

Simple Economy is a subpackage of Simple Kit for building both simple and advanced in‑game economies:

- Currencies modeled as ScriptableObjects with validation and events
- Wallets for storing and modifying currency balances
- Operation results for clear success/error handling

*For requirements check .asmdef*

# Usage

## Creating a currency

Currency is a ScriptableObject describing a resource (name, symbol, etc.), with validation hooks and events.
Create one by extending `CurrencyBase` (auto-created via attribute) and implement checks/events as needed.

```csharp
using Systems.SimpleCore.Operations;
using Systems.SimpleEconomy.Currencies;
using Systems.SimpleEconomy.Data.Context;
using Systems.SimpleEconomy.Operations;
using UnityEngine;

// Assets/Systems/SimpleEconomy/Currencies/DiamondsCurrency.asset is auto-created on first compile
public sealed class DiamondsCurrency : CurrencyBase
{
    // Example validation: cap wallet at 1_000_000
    protected internal override void OnCurrencyAddFailed(
        in CurrencyAddContext context,
        in OperationResult<long> resultAmountExpected)
    {
        Debug.LogError($"Failed to add {resultAmountExpected.data} diamonds to {context.wallet.name}");
    }

    public override OperationResult CanBeAdded(in CurrencyAddContext context)
    {
        const long Max = 1_000_000;
        long future = context.wallet.Balance + context.amount;
        return future <= Max ? EconomyOperations.Permitted() : EconomyOperations.WalletLimitExceeded();
    }
}
```

You can also create abstract currencies to share logic across multiple concrete currencies.

## Creating a wallet

Wallets store balance for a single currency type. Extend `CurrencyWalletBase<TCurrency>` and optionally
override `Add` to enforce custom caps or rules. Attach the wallet component to a GameObject.

```csharp
using Systems.SimpleEconomy.Currencies;
using Systems.SimpleEconomy.Wallets;
using Unity.Mathematics;

public sealed class DiamondsWallet : CurrencyWalletBase<DiamondsCurrency>
{
    // Hard cap at 1_000_000
    protected override long Add(long currencyAmount)
    {
        const long Max = 1_000_000;
        long free = math.max(0, Max - Balance);
        long toAdd = math.min(free, currencyAmount);
        Balance += toAdd;
        return currencyAmount - toAdd; // return leftover that did not fit
    }
}
```

Note: Wallet methods proxy events to the currency (e.g., `OnCurrencyAdded`), unless you override the wallet
event hooks yourself.

## Adding currency

```csharp
using Systems.SimpleCore.Utility.Enums;       // ActionSource
using Systems.SimpleEconomy.Data.Enums;       // ModifyWalletCurrencyFlags
using Systems.SimpleEconomy.Wallets;

var wallet = GetComponent<DiamondsWallet>();
OperationResult<long> left = wallet.TryAdd(100,
    ModifyWalletCurrencyFlags.None,
    ActionSource.External);

bool ok = left;                     // implicit bool via OperationResult
long notAdded = (long)left;         // leftover that did not fit
```

Flags change behavior (e.g., `IgnoreConditions` skips `CanBeAdded`/`CanBeTaken` checks), and `ActionSource.Internal`
prevents event propagation for silent updates.

## Taking currency

```csharp
using Systems.SimpleCore.Utility.Enums;
using Systems.SimpleEconomy.Data.Enums;

OperationResult<long> leftToTake = wallet.TryTake(250,
    ModifyWalletCurrencyFlags.IgnoreConditions,
    ActionSource.Internal);

bool taken = leftToTake;            // true if success
long remaining = (long)leftToTake;  // amount still missing (not taken)
```

Wallets never allow negative balances; `TryTake` will clamp the take amount to available balance.

## Checking balance and conditions

```csharp
bool hasAtLeast500 = wallet.Has(500);

// Preflight checks using contexts (inside overridden methods or before calling TryAdd/TryTake)
// See CurrencyAddContext and CurrencyTakeContext for available fields.
```

## Events and operations

- Currency and wallet expose event hooks:
  - **Added**: `OnCurrencyAdded(context, result)`
  - **Taken**: `OnCurrencyTaken(context, result)`
  - **Add failed**: `OnCurrencyAddFailed(context, resultExpected)`
  - **Take failed**: `OnCurrencyTakeFailed(context, resultExpected)`
- `EconomyOperations` contains common results (e.g., `NotEnoughCurrency`, `CurrencyAdded`, `CurrencyTaken`, `Permitted`).

# Notes

- Use operation results for robust flow control and UI messaging.
- Keep domain‑specific caps and rules either in the currency (`CanBeAdded/CanBeTaken`) or in the wallet (`Add`).
- Prefer `ActionSource.Internal` for silent, system‑driven changes; use `External` to fire gameplay/UI events.
