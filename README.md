# Unity-Cube-Rotation
Class to rotate a cube in any of the four directions.
It creates a lookup table with the rotations and directions fo the rotation. When rolling it takes the proper index of the table to determine the rotation and position.




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

            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                this.cubeRotation.SetDirection(Vector3.forward);
                this.cubeRotation.StartRolling();
            }
            else if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                this.cubeRotation.SetDirection(Vector3.left);
                this.cubeRotation.StartRolling();
            }
            else if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                this.cubeRotation.SetDirection(Vector3.right);
                this.cubeRotation.StartRolling();
            }
            else if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                this.cubeRotation.SetDirection(Vector3.back);
                this.cubeRotation.StartRolling();
            }
            else if(Input.GetKeyDown(KeyCode.Space))
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

