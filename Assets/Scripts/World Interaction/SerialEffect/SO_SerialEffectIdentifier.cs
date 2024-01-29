using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Serial Effect Identifier", menuName = "ScriptableObjects/WorldInteraction/SerialEffectIdentifier")]
public class SO_SerialEffectIdentifier : ScriptableObject
{
    //private UnityEvent serialEffectEvent;
    private ISerialEffect interfaceSerialEffect;

    public void SendSerialEffect()
    {
        SO_SerialEffectIdentifierGroup[] allSEIGroup = Resources.LoadAll<SO_SerialEffectIdentifierGroup>("World Interaction/Serial Effect Identifier Groups");
        Debug.Log("amount of shit: " + allSEIGroup.Length);
        foreach (SO_SerialEffectIdentifierGroup seiGroup in allSEIGroup)
        {
            foreach (SO_SerialEffectIdentifierGroup.Relationship relationship in seiGroup.allRelationships)
            {
                if (relationship.direction == SO_SerialEffectIdentifierGroup.Relationship.Direction.OneWay)
                {
                    if (relationship.FirstUnit == this) { relationship.SecondUnit.ReceiveSerialEffect(this);}
                }
                else
                {
                    if (relationship.FirstUnit == this) { relationship.SecondUnit.ReceiveSerialEffect(this);}
                    if (relationship.SecondUnit == this) { relationship.FirstUnit.ReceiveSerialEffect(this);}
                }
            }
        }
    }

    public void ReceiveSerialEffect(SO_SerialEffectIdentifier origionSEI)
    {
        if(interfaceSerialEffect == null) Debug.LogError("Not set");
        interfaceSerialEffect.ReceiveSerialEffect(origionSEI);
    }

    public void SetUpSerialEffectInterface(ISerialEffect interfaceSerialEffect)
    {
        this.interfaceSerialEffect = interfaceSerialEffect;
    }
}
