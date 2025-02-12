#include <iostream>

class SensorData {
public:
    int temperature;

    SensorData(int temp) : temperature(temp) {
        std::cout << "SensorData created with temperature: " << temperature << std::endl;
    }

    void printTemperature() {
        std::cout << "Current temperature: " << temperature << std::endl;
    }

    ~SensorData() {
        std::cout << "SensorData object destroyed!" << std::endl;
    }
};

void processData(SensorData* data) {
    // Simulate processing data
    if (data) {
        data->printTemperature();
    } else {
        std::cout << "Invalid SensorData pointer." << std::endl;
    }
}

int main() {
    SensorData* sensor1 = new SensorData(25);
    processData(sensor1);
    delete sensor1;

      

    SensorData* sensor2 = new SensorData(15);
    processData(sensor1);    
    delete sensor2;


           
    if (sensor1 != nullptr) {
         sensor1 = nullptr;
    }
    if (sensor2 != nullptr) {
         sensor2 = nullptr;
    }
    


    return 0;
}
