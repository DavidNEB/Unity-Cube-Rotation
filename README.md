# Unity-Cube-Rotation
Class to rotate a cube in any of the four directions.




Example:

    public class RollingCube : MonoBehaviour
    {
        Utils.CubeRotation cubeRotation;

        void Start()
        {
            this.cubeRotation = new Utils.CubeRotation(this.transform, Vector3.back, 180);
        }

        void Update()
        {
            this.cubeRotation.Roll(Time.deltaTime);

            // toggle rolling with the space key
            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(this.cubeRotation.rolling)
                {
                    this.cubeRotation.StopRolling();
                }
                else
                {
                    this.cubeRotation.StartRolling();
                }
            }
        }
    }
