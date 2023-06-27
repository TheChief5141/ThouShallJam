using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class howtousedithering : MonoBehaviour
{
    //Hi welcome to my TED talk I'll be walking you through how to apply and change the dithering effect settings to get it just the way you want
    
    //First thing if URP is applied in the project, remove it as it just don't be working ya know

    //Second thing, all effects that are applied are only visable in the Game View so press play to see your changes! Changes can be done in realtime but make sure
    //you keep track of your changes just in case you like them as once you leave play mode they won't be saved. I usually just screenshot my whole monitor.

    //Okay here's the real meat and potatoes. In the "Pixel Art" folder you will find a "Ditherer" script, apply this script to the main camera. After applying
    //drag and drop the "Dither" shader (it has the big S and it right next to Ditherer script in the Pixel Art folder) in the main camera ditherer script where it
    //says "dither shader". 

    //Here's where the fun begins
    //You will see a few sliders. Set the "Down Samples" to 2 and "Bayer level" to 1. The rest is up to you. 

    //Here's a solid starting off point:
    //Spread - 0.25
    //Red Count - 4
    //Green Count - 3
    //Blue count - 7
    //Bayer Level - 1
    //Down Samples - 2
    //Point filter down - unchecked

    //There are a ton of other folders in this 
}
