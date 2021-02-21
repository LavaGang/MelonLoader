//Copyright bHaptics Inc. 2017
#ifndef BHAPTICS_MODEL
#define BHAPTICS_MODEL

#include <map>
#include <vector>
#include <string>

namespace bhaptics
{

    using namespace std;

    enum PositionType {
        All = 0, Left = 1, Right = 2,
        Vest = 3,
        Head = 4,
        Racket = 5,
        HandL = 6, HandR = 7,
        FootL = 8, FootR = 9,
        ForearmL = 10, ForearmR = 11,
        VestFront = 201, VestBack = 202,
        GloveLeft = 203, GloveRight = 204,
        Custom1 = 251, Custom2 = 252, Custom3 = 253, Custom4 = 254
    };

    enum FeedbackMode {
        PATH_MODE,
        DOT_MODE
    };

    struct DotPoint
    {
        int index;
        int intensity;
        DotPoint(int _index, int _intensity)
        {
            index = _index;
            if (_index < 0)
                index = 0;
            else if (_index > 19)
                index = 19;
            intensity = _intensity;
        }

        std::string to_string()
        {
            std::string ret = "{ \"Index\":" + std::to_string(index) + ", \"Intensity\": " + std::to_string(intensity) + "}";
            return ret;
        }
    };

    struct PathPoint
    {
        float x;
        float y;
        int intensity;
        int MotorCount;

        PathPoint(float _x, float _y, int _intensity, int motorCount = 3) {
            x = _x;
            y = _y;
            intensity = _intensity;
            MotorCount = motorCount;
        }

        PathPoint(int _x, int _y, int _intensity, int motorCount = 3)
        {
            int xRnd =_x;
            int yRnd = _y;
            intensity = _intensity;
            if (motorCount < 1)
            {
                MotorCount = 1;
            }
            else if (motorCount > 3)
            {
                MotorCount = 3;
            }
            else
            {
                MotorCount = motorCount;
            }

            if (xRnd < 0)
            {
                xRnd = 0;
            }
            else if (xRnd > 1000)
            {
                xRnd = 1000;
            }

            if (yRnd < 0)
            {
                yRnd = 0;
            }
            else if (yRnd > 1000)
            {
                yRnd = 1000;
            }

            x = (float)(xRnd) / 1000;
            y = (float)(yRnd) / 1000;
        }

        std::string to_string()
        {
            std::string ret = "{ \"X\":" + std::to_string(x) + ", \"Y\": " + std::to_string(y) + ", \"Intensity\": " + std::to_string(intensity) + ", \"MotorCount\": " + std::to_string(MotorCount) + "}";
            return ret;
        }

    };

    struct HapticFile
    {
        int intervalMillis;
        int size;
        int durationMillis;
        std::string ProjectJson;
    };

    class HapticFrame
    {
    public:
        int DurationMillis = 0;
        PositionType Position;
        std::vector<PathPoint> PathPoints;
        std::vector<DotPoint> DotPoints;
        int Texture;

        static HapticFrame AsPathPointFrame(std::vector<PathPoint> points, bhaptics::PositionType position, int durationMillis, int texture = 0)
        {
            HapticFrame frame;
            frame.Position = position;
            frame.PathPoints = points;
            frame.Texture = texture;
            frame.DurationMillis = durationMillis;
            return frame;
        }

        static HapticFrame AsDotPointFrame(std::vector<DotPoint> points, bhaptics::PositionType position, int durationMillis, int texture = 0)
        {
            HapticFrame frame;
            frame.Position = position;
            frame.DotPoints = points;
            frame.Texture = texture;
            frame.DurationMillis = durationMillis;
            return frame;
        }

        std::string to_string()
        {
            std::vector<std::string> tempVec1, tempVec2;
            std::string pathArrayStr, dotArrayStr;
            pathArrayStr = "[";
            for (size_t i = 0; i < PathPoints.size(); i++)
            {
                std::string tmp = PathPoints[i].to_string();
                pathArrayStr.append(tmp);
                if (i + 1 < PathPoints.size())
                {
                    pathArrayStr.append(",");
                }
            }
            pathArrayStr.append("]");

            dotArrayStr = "[";
            for (size_t i = 0; i < DotPoints.size(); i++)
            {

                std::string tmp = DotPoints[i].to_string();
                dotArrayStr.append(tmp);
                if (i + 1 < DotPoints.size())
                {
                    dotArrayStr.append(",");
                }
            }
            dotArrayStr.append("]");

            std::string ret = "{ \"DurationMillis\":" + std::to_string(DurationMillis) + ", \"Position\": " + std::to_string(Position) + ", \"Texture\": " + std::to_string(Texture) +
                ", \"DotPoints\": " + dotArrayStr + ", \"PathPoints\": " + pathArrayStr + "}";
            return ret;
        }
    };

    struct ScaleOption
    {
        float Intensity;
        float Duration;

        std::string to_string()
        {
            std::string ret = "{ \"intensity\" : " + std::to_string(Intensity) + ", \"duration\" : " + std::to_string(Duration) + "}";
            return ret;
        }
    };

    struct RotationOption
    {
        float OffsetAngleX;
        float OffsetY;

        std::string to_string()
        {
            std::string ret = "{ \"offsetAngleX\" : " + std::to_string(OffsetAngleX) + ", \"offsetY\" : " + std::to_string(OffsetY) + "}";
            return ret;
        }
    };


    struct RegisterRequest
    {
        std::string Key;
        std::string ProjectJson;

        std::string to_string()
        {
            std::string ret = "{ \"Key\" : \"" + Key + "\", \"Project\" : " + ProjectJson + "}";
            return ret;
        }
    };

    struct SubmitRequest
    {
        std::string Type;
        std::string Key;
        HapticFrame Frame;
        std::map<std::string, std::string> Parameters;

        std::string to_string()
        {
            std::string paramMapStr = "{";
            for (auto& p : Parameters)
            {
                paramMapStr.append("\"" + p.first + "\": " + p.second + ",");
            }
            paramMapStr.pop_back();
            paramMapStr.append("}");

            std::string paramString = "";
            if (paramMapStr.size() > 2) {
                paramString = ", \"Parameters\": " + paramMapStr;
            }

            std::string ret = "{ \"Type\" : \"" + Type + "\", \"Key\" : \"" + Key + "\"" + paramString + ", \"Frame\" : " + Frame.to_string() + " }";
            return ret;
        }
    };

    class PlayerRequest
    {
    public:
        std::vector<RegisterRequest> Register;
        std::vector<SubmitRequest> Submit;

        static PlayerRequest* Create()
        {
            return new PlayerRequest();
        }

        std::string to_string()
        {
            std::string registerArrayStr, submitArrayStr;

            registerArrayStr = "[";
            for (size_t i = 0; i < Register.size(); i++)
            {
                registerArrayStr.append(Register[i].to_string());
                if (i + 1 < Register.size())
                {
                    registerArrayStr.append(",");
                }
            }
            registerArrayStr.append("]");

            submitArrayStr = "[";
            for (size_t i = 0; i < Submit.size(); i++)
            {
                submitArrayStr.append(Submit[i].to_string());
                if (i + 1 < Submit.size())
                {
                    submitArrayStr.append(",");
                }
            }
            submitArrayStr.append("]");

            std::string ret = "{ \"Register\" : " + registerArrayStr + ", \"Submit\" : " + submitArrayStr + "}";
            return ret;
        }
    };

    struct HapticFeedback
    {
        HapticFeedback(PositionType DevicePosition, vector<int> &Values) :
            DevicePosition(DevicePosition),Values(Values) {}

        PositionType DevicePosition;
        std::vector<int>& Values;
    };

}

#endif