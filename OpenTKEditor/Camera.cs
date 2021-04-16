using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics.OpenGL;
namespace OpenTKEditor
{
    class Camera
    {
        public float camX { get; set; }
        public float camZ { get; set; }
        private float cameraSpeed = 0.03f;
        public float cameraSpeed_out = 2.5f;
        private float _yaw, _pitch, _roll = 90.0f;
        private float degToRad = (float)(2 * Math.PI / 180);
        // Original camera position vector

        // Vector that defines where the camera is pointing at
        private Vector3 cameraTarget = new Vector3(0.0f, 0.0f, 0.0f);
        private Vector3 cameraDirection = new Vector3(0.0f, 0.0f, 0.0f);

        private Vector3 up = new Vector3(0.0f, 0.0f, 1.0f);
        private Vector3 cameraRight = new Vector3(1.0f, 0.0f, 0.0f);

        private Vector3 cameraPos = new Vector3(0.0f, 0.0f, 3.0f);
        private Vector3 cameraFront = new Vector3(0.0f, 0.0f, -1.0f);
        public Vector3 cameraUp = new Vector3(0.0f, 1.0f, 0.0f);

        float roll_x = 0.0f;
        float roll_y = 0.0f;

        public Camera()
        {
            _yaw = 0.0f;
            _roll = 0.0f;
            _pitch = 0.0f;
            UpdateCamera();
        }
        public void MoveUpDown(float input)
        {
            cameraPos += input * cameraFront;
        }
        public void MoveUp(float input)
        {
            cameraPos += input * cameraFront;
        }
        public void MoveDown(float input)
        {
            cameraPos -= input * cameraFront;
        }
        public void MoveLeft()
        {
            // Cross product of up and front vectors
            cameraPos -= Vector3.Normalize(Vector3.Cross(cameraFront, cameraUp)) * cameraSpeed_out;
        }
        public void MoveRight()
        {
            cameraPos += Vector3.Normalize(Vector3.Cross(cameraFront, cameraUp)) * cameraSpeed_out;
        }

        public Matrix3 GetViewMatrix()
        {

            return new Matrix3(cameraPos, cameraPos + cameraFront, cameraUp)
                * Matrix3.CreateRotationZ(roll_x) * Matrix3.CreateRotationY(roll_y);
        }

        public void SetCameraJitter(float dt)
        {
            cameraSpeed_out = cameraSpeed * dt;
        }

        public void SetYawPitchRoll(float yaw, float pitch, float roll)
        {
            _yaw += yaw;
            //if (_yaw < -89.9f)
            //    _yaw = -89.9f;
            //else if (_yaw > 89.9f)
            //    _yaw = 89.9f;

            _pitch += pitch;
            //if (_pitch < -89.9f)
            //    _pitch = -89.9f;
            //else if (_pitch > 89.9f)
            //    _pitch = 89.9f;

            _roll += roll;
            //if (_roll < -89.9f)
            //    _roll = -89.9f;
            //else if (_roll > 89.9f)
            //    _roll = 89.9f;

            UpdateCamera();
        }
        public void UpdateCamera()
        {
            Vector3 front = new Vector3((float)Math.Cos(degToRad * _yaw) * (float)Math.Cos(degToRad * _pitch),
                    (float)Math.Sin(degToRad * _pitch),
                    (float)Math.Sin(degToRad * _yaw) * (float)Math.Cos(degToRad * _pitch));

            cameraFront = Vector3.Normalize(front);
        }
        public void Roll(float alpha, float beta, float gam)
        {
            roll_x += alpha;
            roll_y += beta;
        }
    }
}