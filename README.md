# Simple Economy
## About
Simple Economy is a subpackage of SimpleKit intended for quick and easy implementation of a basic
and more complicated economy.

*For requirements check .asmdef*

## Creating a currency
Currency is an information object that is used to store basic currency info (such as name, symbol, etc.) and events.

To create a currency simply extend `CurrencyBase` class (it's an Auto-Created ScriptableObject) and implement
desired checks and events. For reference of all available events and checks see `CurrencyBase` class.

## Creating a wallet
Wallet is storage of currencies. To create a wallet simply extend `CurrencyWalletBase<TCurrency>` class
and implement desired events / methods. 

Beware that overriding wallet methods may disable calls to 
currency-level events as they're proxy-passed from wallet events to currency class.

Wallets after creation can be added to GameObjects to make them store currencies.