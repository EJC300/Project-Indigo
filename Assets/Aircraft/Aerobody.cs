using UnityEngine;

public class Aerobody : MonoBehaviour
{
    /*
     * Instead of torque being applied directly torque is applied based on 
     * an abstract lift deflection of ailerons,rudder,and elevators/flaperons 
     * Instead of acting like thing in physical place the control surfaces act like a value between 1 and -1 with a damage efficiency meter applied to lift.
     * Torque itself uses lift in the calculation of torque lift like in real world produce a little bit of torque.
     * As such there are directions that affect pitch yaw and roll but they are multiplied by lift. These are small values
     * basically directions multiplied by lift 
     * these are summed up and put on the torque
     * 
     * 
     * 
     * An example would be like roll left roll right
     * left wing
     * damageEfficiency = 1.0( full wing) - 0.5 (damaged wing) - 0 (no wing)!
     * rollDirection = damageEfficiency * cl * (input + 0.5f)
     * right wing
     * damageEfficiency = 1.0( full wing) - 0.5 (damaged wing) - 0 (no wing)!
     * rollDirection = damageEfficiency * cl * (input + 0.5f)
     * ----
     * plane roll = rollDirections * cl
     * 
     * negative roll rollLeft
     * positive rollDireciton rollRight
     * 
     * --apply torque with that roll
     * 
     */
}
