using Systems.SimpleCore.Operations;
using Systems.SimpleEconomy.Data;
using UnityEngine;

namespace Systems.SimpleEconomy.Examples.Scripts
{
    [DisallowMultipleComponent]
    public sealed class ExampleEconomyScene : MonoBehaviour
    {
        [SerializeField] private long _grantAmount = 100L;
        [SerializeField] private long _spendAmount = 35L;

        private ExampleGoldWallet _wallet;

        private void Awake()
        {
            if (!TryGetComponent(out _wallet))
                _wallet = gameObject.AddComponent<ExampleGoldWallet>();
        }

        private void Start()
        {
            RunExample();
        }

        [ContextMenu("Run Economy Example")]
        public void RunExample()
        {
            ExampleGoldCurrency currency = CurrencyDatabase.GetExact<ExampleGoldCurrency>();
            if (ReferenceEquals(currency, null))
            {
                Debug.LogWarning("[SimpleEconomy] ExampleGoldCurrency was not found in the currency database. Let the auto-create/addressables setup generate it before running wallet operations.");
                return;
            }

            OperationResult grantResult = _wallet.TryAdd(_grantAmount);
            OperationResult spendResult = _wallet.TryTake(_spendAmount);
            Debug.Log("[SimpleEconomy] Grant result: " + grantResult + ", spend result: " + spendResult + ", final balance: " + _wallet.Balance);
        }
    }
}
