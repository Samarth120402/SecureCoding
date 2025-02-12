#include <iostream>
#include <thread>
#include <mutex>

int counter = 0;
std::mutex counterMutex;

void increment() {
    for (int i = 0; i < 1000000; ++i) {
        std:: lock_guard<std::mutex> lock(counterMutex);
        ++counter;
    }
}

int main() {
    std::thread t1(increment);
    std::thread t2(increment);

    t1.join();
    t2.join();

    std::cout << "Counter: " << counter << std::endl;
    return 0;
}

