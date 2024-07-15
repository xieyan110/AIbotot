import subprocess
import re
import time
import random
import string
from paddleocr import PaddleOCR
import os


# ADB 路径，根据实际情况修改
ADB_PATH = "adb"

ocr = PaddleOCR(use_angle_cls=True, lang="ch")


sdcard_dir = './sdcard'
if not os.path.exists(sdcard_dir):
    os.makedirs(sdcard_dir)

# 执行ADB命令
def run_adb_command(command):
    try:
        subprocess.run([ADB_PATH] + command, check=True)
    except subprocess.CalledProcessError as e:
        print("错误：无法执行ADB命令")
        # 可以添加其他错误处理逻辑

# 杀死某个应用
def kill_app(package_name):
    run_adb_command(["shell", "am", "force-stop", package_name])
    print("成功杀死应用:", package_name)

# 打开某个应用
def open_app(package_name):
    run_adb_command(["shell", "monkey", "-p", package_name, "-c", "android.intent.category.LAUNCHER", "1"])
    print("成功打开应用:", package_name)

# 等待多长时间
def wait(seconds):
    print("等待", seconds, "秒...")
    time.sleep(seconds)

# 通过文字获取坐标
def get_coordinates(text, position="last"):
    output = subprocess.check_output([ADB_PATH, "shell", "input", "text", text]).decode().strip()
    if position == "last":
        match = re.search(r"(\d+),(\d+)", output)
        if match:
            x, y = match.groups()
            return int(x), int(y)
        else:
            print("错误：未能解析坐标")
            return None, None
    elif position == "first":
        # 实现获取第一个匹配坐标的逻辑
        pass
    else:
        print("错误：不支持的位置参数")
        return None, None

# 输入文字
def is_app_installed(package_name):
    """判断应用是否已安装"""
    output = subprocess.check_output([ADB_PATH, "shell", "pm", "list", "packages", "-f", package_name])
    return package_name in output.decode()

def install_app(apk_path):
    """安装应用"""
    run_adb_command(["install", "-r", apk_path])

def get_default_ime():
    """获取当前默认输入法"""
    output = subprocess.check_output([ADB_PATH, "shell", "settings", "get", "secure", "default_input_method"]).decode().strip()
    return output

def switch_ime(ime):
    """切换输入法"""
    run_adb_command(["shell", "ime", "set", ime])

def input_text(text):
    """输入文字"""
    # 获取当前默认输入法
    default_ime = get_default_ime()

    # 判断是否已安装 ADBKeyboard
    adbkeyboard_package = "com.android.adbkeyboard"
    if not is_app_installed(adbkeyboard_package):
        # 未安装则安装
        apk_path = os.path.join(os.getcwd(), "ADBKeyboard", "ADBKeyboard.apk")
        if os.path.exists(apk_path):
            install_app(apk_path)
        else:
            print("错误: ADBKeyboard APK 文件不存在")
            return

    # 切换到 ADBKeyboard 输入法
    switch_ime(f"{adbkeyboard_package}/.AdbIME")
    wait(0.5)
    # 输入文本
    run_adb_command(["shell", "am", "broadcast", "-a", "ADB_INPUT_TEXT", "--es", "msg", text])
    print(f"成功输入文字: {text}")

    # 切换回原来的输入法
    switch_ime(default_ime)

# 返回
def go_back():
    run_adb_command(["shell", "input", "keyevent", "KEYCODE_BACK"])
    print("成功执行返回操作")

def swipe(direction, start_x, start_y, move_y, duration):
    """
    执行滑动操作
    
    Args:
        direction (str): 滑动方向，'up'表示上滑，'down'表示下拉
        start_x (int): 起始点x坐标
        start_y (int): 起始点y坐标
        move_y (int): y移动多少
        duration (int): 滑动持续时间（毫秒）
    """
    if direction == 'up':
        run_adb_command(["shell", "input", "swipe", str(start_x), str(start_y), str(start_x), str(start_y + move_y), str(duration)])
        print("成功执行上滑操作")
    elif direction == 'down':
        run_adb_command(["shell", "input", "swipe", str(start_x), str(start_y), str(start_x), str(start_y + move_y), str(duration)])
        print("成功执行下拉操作")
    else:
        print("错误：不支持的滑动方向")


# 长按
def long_press(x, y, duration=1):
    run_adb_command(["shell", "input", "swipe", str(x), str(y), str(x), str(y), str(duration * 1000)])
    print("成功执行长按操作")

# 返回ADB设备执行列表
def get_adb_device_list():
    output = subprocess.check_output([ADB_PATH, "devices"]).decode()
    devices = re.findall(r"(\S+)\s+device", output)
    if devices:
        return devices
    else:
        print("未检测到连接的ADB设备")
        return []

# 截图并保存到本地
def generate_random_filename(length=10):
    # 生成指定长度的随机字符串作为文件名
    return ''.join(random.choices(string.ascii_letters + string.digits, k=length)) + '.png'

def screenshot():
    # 生成随机的图片名称
    filename = generate_random_filename()
    
    # 执行截图命令
    run_adb_command(['shell', 'screencap', '-p', '/sdcard/' + filename])
    
    # 将截图文件拉取到本地当前目录下
    run_adb_command(['pull', '/sdcard/' + filename, './sdcard/' + filename])
    
    # 返回图片名称的路径
    return './sdcard/' + filename



# 使用 PaddleOCR 进行 OCR 识别，正序倒叙 asc和desc 
def find_text_coordinates(text, order="asc", index=0):
    
    img_path = screenshot()
    results = ocr.ocr(img_path, cls=True)[0]
    if order != "asc":
        results = results[::-1]
    text_coordinates = []
    for result in results:
        value,_ = result[1]
        if value in text:
            text_coordinates.append(result[0])
    return text_coordinates[index]

#文字定位中间坐标
def middle_center(coordinates):
    if coordinates:
        x1, y1 = coordinates[0]
        x2, y2 = coordinates[2]
        return (x1 + x2) / 2, (y1 + y2) / 2
    return None

#文字定位最右坐标
def right_most_center(coordinates):
    if coordinates:
        x1, y1 = coordinates[0]
        x2, y2 = coordinates[2]
        x3, y3 = coordinates[3]
        return (x2 + x3) / 2, (y2 + y3) / 2
    return None

#文字定位最左坐标
def left_most_center(coordinates):
    if coordinates:
        x1, y1 = coordinates[0]
        x2, y2 = coordinates[1]
        return (x1 + x2) / 2, (y1 + y2) / 2
    return None

# 实现聊天记录解析逻辑
def is_valid_message(content):
    message = content[0]
    if re.match(r'^\d+:\d+$', message) or re.match(r'^\\(.*\\)$', message):
        return False
    return True

def parse_chat_record(record):
        coord, content = record
        if not is_valid_message(content):
            return None
        if coord[0][0] < 60:
            return None

        sender = 'other'
        if coord[0][0] >= 270:
            sender = 'me'

        message = content[0]
        time_str = re.search(r'\d+:\d+', message)
        if time_str:
            time = time_str.group()
        else:
            time = None

        return {
            'sender': sender,
            'message': message,
            'time': time
        }

# 聊天记录，智能解析
def parse_chat_records(chat_records):

    parsed_records = [parse_chat_record(record) for record in chat_records if parse_chat_record(record)]
    return parsed_records

# 聊天记录存储，存储用户
def store_chat_records(records):
    # 实现聊天记录存储逻辑
    pass

# 聊天记录判断是否包括
def contains_chat_records(records, keyword):
    # 实现聊天记录包含关键字判断逻辑
    pass