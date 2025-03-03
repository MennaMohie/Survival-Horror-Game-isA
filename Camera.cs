using GlmNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Graphics
{
    class Camera
    {
        float mAngleX = 0;
        float mAngleY = 0;
        vec3 mDirection;
        public vec3 mPosition;
        vec3 mRight;
        vec3 mUp;
        mat4 mViewMatrix;
        mat4 mProjectionMatrix;
        float PersonHeight;
        public vec3 PersonBoundingBox = new vec3(0, 0, 0);
        public static bool Move = false;

        public Camera()
        {
            Reset(0, 0, 5, 0, 0, 0, 0, 1, 0);
            SetProjectionMatrix(45, 4 / 3, 0.1f, 1000);
        }

        public vec3 GetLookDirection()
        {
            return mDirection;
        }

        public mat4 GetViewMatrix()
        {
            return mViewMatrix;
        }

        public mat4 GetProjectionMatrix()
        {
            return mProjectionMatrix;
        }
        public vec3 GetCameraPosition()
        {
            return mPosition;
        }
        public void Reset(float eyeX, float eyeY, float eyeZ, float centerX, float centerY, float centerZ, float upX, float upY, float upZ)
        {
            vec3 eyePos = new vec3(eyeX, eyeY, eyeZ);
            vec3 centerPos = new vec3(centerX, centerY, centerZ);
            vec3 upVec = new vec3(upX, upY, upZ);

            mPosition = eyePos;
            mDirection = centerPos - mPosition;
            mRight = glm.cross(mDirection, upVec);
            mUp = upVec;
            mUp = glm.normalize(mUp);
            mRight = glm.normalize(mRight);
            mDirection = glm.normalize(mDirection);

            mViewMatrix = glm.lookAt(mPosition, centerPos, mUp);

            PersonHeight = eyeY;
            PersonBoundingBox.y = PersonHeight;
        }

        public void UpdateViewMatrix()
        {
            mDirection = new vec3((float)(-Math.Cos(mAngleY) * Math.Sin(mAngleX))
                , (float)(Math.Sin(mAngleY))
                , (float)(-Math.Cos(mAngleY) * Math.Cos(mAngleX)));
            mRight = glm.cross(mDirection, new vec3(0, 1, 0));
            mUp = glm.cross(mRight, mDirection);

            vec3 center = mPosition + mDirection;

            mViewMatrix = glm.lookAt(mPosition, center, mUp);
        }
        public void SetProjectionMatrix(float FOV, float aspectRatio, float near, float far)
        {
            mProjectionMatrix = glm.perspective(FOV, aspectRatio, near, far);
        }


        public void Yaw(float angleDegrees)
        {
            mAngleX += angleDegrees;
        }

        public void Pitch(float angleDegrees)
        {
            mAngleY += angleDegrees;
        }

        public bool checkPos(float dist, short axis)
        {
            vec3 result;

            if (axis == 0)      //Z
                result = mDirection;
            else if (axis == 1) //X
                result = mRight;
            else                //Y
                result = mUp;

            result *= dist;
            result += mPosition;
            return (result.x >= 3 && result.x <= Renderer.skyboxes[Renderer.currentSkyboxID].maxX &&
                    result.y >= 3 && result.y <= Renderer.skyboxes[Renderer.currentSkyboxID].maxY &&
                    result.z >= 3 && result.z <= Renderer.skyboxes[Renderer.currentSkyboxID].maxZ);
        }

        public bool checkCollision(float dist, short axis)
        {
            float DistanceX, DistanceY, DistanceZ;

            vec3 result;

            if (axis == 0)      //Z
                result = mDirection;
            else if (axis == 1) //X
                result = mRight;
            else                //Y
                result = mUp;
            result *= dist;
            result += mPosition;

            result.y /= 2;

            for (int i = 0; i < Renderer.Models_3D.Count; i++)
            {
                if (!Renderer.Models_3D[i].isDrawn)
                    continue;
                DistanceX = Math.Abs(result.x - Renderer.Models_3D[i].position.x);
                DistanceY = Math.Abs(result.y - Renderer.Models_3D[i].position.y);
                DistanceZ = Math.Abs(result.z - Renderer.Models_3D[i].position.z);
                if (DistanceX < (PersonBoundingBox.x + Renderer.Models_3D[i].collisionBoundingBox.x) / 2
                 && DistanceY < (PersonBoundingBox.y + Renderer.Models_3D[i].collisionBoundingBox.y) / 2
                 && DistanceZ < (PersonBoundingBox.z + Renderer.Models_3D[i].collisionBoundingBox.z) / 2)
                    return false;
            }
            for (int i = 0; i < Renderer.Models_Interactive.Count; i++) {
                if (!Renderer.Models_Interactive[i].obj.isDrawn)
                    continue;
                DistanceX = Math.Abs(result.x - Renderer.Models_Interactive[i].obj.position.x);
                DistanceY = Math.Abs(result.y - Renderer.Models_Interactive[i].obj.position.y);
                DistanceZ = Math.Abs(result.z - Renderer.Models_Interactive[i].obj.position.z);
                if (DistanceX < (PersonBoundingBox.x + Renderer.Models_Interactive[i].obj.collisionBoundingBox.x) / 2
                 && DistanceY < (PersonBoundingBox.y + Renderer.Models_Interactive[i].obj.collisionBoundingBox.y) / 2
                 && DistanceZ < (PersonBoundingBox.z + Renderer.Models_Interactive[i].obj.collisionBoundingBox.z) / 2)
                    return false;
            }
            return true;
        }

        public void Walk(float dist)
        {
            if (!checkPos(dist, 0) || !checkCollision(dist, 0) || Renderer.text_shown)
            {
                Move = false;
                return;
            }
            mPosition += dist * mDirection;
            mPosition.y = PersonHeight;
            Move = true;
        }
        public void Strafe(float dist)
        {
            if (!checkPos(dist, 1) || !checkCollision(dist, 1) || Renderer.text_shown)
            {
                Move = false;
                return;
            }
            mPosition += dist * mRight;
            mPosition.y = PersonHeight;
            Move = true;
        }
        public void Fly(float dist)
        {
            if (!checkPos(dist, 2) || !checkCollision(dist, 2) || Renderer.text_shown)
            {
                Move = false;
                return;
            }
            mPosition += dist * mUp;
            mPosition.y = PersonHeight;
            Move = true;
        }
    }
}
