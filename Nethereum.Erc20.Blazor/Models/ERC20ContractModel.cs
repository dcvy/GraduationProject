using FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Nethereum.Erc20.Blazor
{
    public class ERC20ContractModel
    {
        public const int DEFAULT_DECIMALS = 18;
        public const string DAI_SMART_CONTRACT = "0xf39fd6e51aad88f6f4ce6ab8827279cfffb92266";
        public ERC20ContractModel() { }

        public string Address { get; set; } = DAI_SMART_CONTRACT;
        public int DecimalPlaces { get; set; } = DEFAULT_DECIMALS;
        public string Name { get; set; }
    }
   
}

