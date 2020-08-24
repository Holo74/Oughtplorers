///<summary>What type of damage an entity takes when they get damaged</summary>
public enum DamageType
{
    debug, //Everything should take this damage and take it without lowering the damage
    basic, //This is the most basic damage that everything will deal and take
    posion, //Posion fog and other creatures that deal posion damage will use this
    fall //Damage that is taken when falling from a certain distance
}