# Simple Interactions
Simple Interactions is an interaction system that focusses on keeping it simple. It is mostly meant for prototypes where it is handy for players to interact with object. For example you are creating particle effects and you have a demo game were people can see them in action. You can use this library so people can press buttons that activate your particles.

Only first person is supported using the build in player controller.

Feel free to fork this and make your own changes. I hope it helps you out!

# How to use
Once installed you can go to `add new component` and select `New component`. Under `Create Script From Template` select `New SimpleInteraction` component.

`OnInteract()` is called when the player interacts with the object. You can use this to do whatever you want. For example, you can make a door open, play a sound, or spawn a particle effect.

`InteractionString` is the text that is displayed when the player can interact with the object. For example, you can set this to 'Open door'. Available in the editor.

`InteractionEnabled` is a boolean that you can use to enable or disable the interaction. For example, you can set this to false when the door is already open. Available in the editor.

`InteractionDistance` is the distance the player has to be from the object to interact with it. Available in the editor.

`InteractionHold` is a boolean that you can use to make the player hold the interaction button to interact with the object. Available in the editor.

`InteractionHoldDuration` is the duration the player has to hold the interaction button to interact with the object. Available in the editor.

`Collider` is the collider the interaction system uses to check if the player is looking at the object. This is automatically set to the object's collider. But you can override this by setting it to another collider. Within the editor. Available in the editor.

# Demo
I made a demo scene to show you how to use the library. You can find it [here](https://sbox.game/guusconl/simple_interactions_demo)