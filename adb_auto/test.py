import subprocess
import re
import cv2
import numpy as np
from paddleocr import PaddleOCR

# 执行 adb 命令的函数
def run_adb(command):
    process = subprocess.Popen(command, stdout=subprocess.PIPE, stderr=subprocess.PIPE, shell=True)
    output, error = process.communicate()
    return output.decode('utf-8'), error.decode('utf-8')

# 截图并保存到本地
def screenshot():
    run_adb('adb shell screencap -p /sdcard/screen.png')
    run_adb('adb pull /sdcard/screen.png .')

# 启动微信应用
run_adb('adb shell monkey -p com.tencent.mm -c android.intent.category.LAUNCHER 1')

while True:
    # 检查微信是否在前台运行
    output, _ = run_adb('adb shell dumpsys window windows')
    if 'com.tencent.mm' not in output:
        print('微信未在前台运行')
        continue

    # 截图
    screenshot()

    # 使用 PaddleOCR 进行 OCR 识别
    ocr = PaddleOCR(use_angle_cls=True, lang="ch")
    img_path = 'screen.png'
    result = ocr.ocr(img_path, cls=True)
    for line in result[0]:
        print(line)

