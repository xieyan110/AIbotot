import sqlite3

from adb_fn import *

# 数据库连接和表创建
conn = sqlite3.connect('chat_records.db')
c = conn.cursor()
c.execute('''CREATE TABLE IF NOT EXISTS chat_records
             (customer_name TEXT, message TEXT, sender TEXT, timestamp TEXT)''')

SEND_MESSAGE = ""
# 从数据库加载客户数据
def load_customer_data():
    # 从数据库或其他数据源加载客户数据
    customer_data = [
        {'name': '阿呆', 'honorific': '先生', 'info': '客户阿呆的信息', 'need_chat': True},
        {'name': '小明', 'honorific': '女士', 'info': '客户小明的信息', 'need_chat': True},
        # 添加更多客户数据
    ]
    return customer_data

# 更新数据库中的聊天记录
def update_chat_records(customer_name, message, sender, timestamp):
    c.execute("INSERT INTO chat_records VALUES (?, ?, ?, ?)", (customer_name, message, sender, timestamp))
    conn.commit()

# 检查是否有新的客户回复
def check_new_replies():
    # 实现检查主页上是否有新的客户回复的逻辑
    pass

# 规则判断是否应该回复
def should_reply(message):
    # 实现判断是否应该回复的规则逻辑
    return True

# 生成AI回复
def generate_ai_reply(customer_name, customer_honorific, customer_info, chat_history):
    # 调用AI模型生成回复
    ai_reply = f"你好{customer_name}{customer_honorific}，根据{customer_info}..."
    return ai_reply

# 主循环
customer_data = load_customer_data()
processed_customers = set()


# 1. 杀死微信
kill_app('com.tencent.mm')

# 2. 打开微信,等待
open_app('com.tencent.mm')
wait(1)

# 3. 图片识别出通讯录
contact_list_coords = find_text_coordinates('通讯录',"desc")
if not contact_list_coords:
    print("未能识别出通讯录入口")
    

# 4. 从后往前数,第一个通讯录就是
contact_list_center = right_most_center(contact_list_coords)
run_adb_command(command=['shell', 'input', 'tap', str(contact_list_center[0]), str(contact_list_center[1])])

for customer in customer_data:
    if customer['name'] in processed_customers:
        continue
    
    # 5. 图片识别出需要操作的客户
    wait(0.5)
    customer_coords = find_text_coordinates(customer['name'])
    if not customer_coords:
        continue
    
    # 6. 点击客户
    customer_center = middle_center(customer_coords)
    run_adb_command(['shell', 'input', 'tap', str(customer_center[0]), str(customer_center[1])])
    wait(1)
    # 7. 识别出"发消息"并点击
    send_message_coords = find_text_coordinates('发消息')
    if not send_message_coords:
        go_back()
        continue
    send_message_center = middle_center(send_message_coords)
    run_adb_command(['shell', 'input', 'tap', str(send_message_center[0]), str(send_message_center[1])])
    
    # 8. 获取聊天记录
    wait(1)
    chat_records = ocr.ocr(screenshot(), cls=True)[0]
    
    parsed_records = parse_chat_records(chat_records)
    
    # 9. 生成AI回复并发送
    ai_reply = generate_ai_reply(customer['name'], customer['honorific'], customer['info'], parsed_records)
    
    run_adb_command(['shell', 'input', 'tap', str(send_message_center[0]), str(send_message_center[1])])
    # 激活输入焦点
    wait(0.5)
    try:
        send_message_coords = find_text_coordinates('6の',"desc")
        if not send_message_coords:
            go_back()
            continue
        send_message_center = middle_center(send_message_coords)
        run_adb_command(['shell', 'input', 'tap', str(send_message_center[0]), str(send_message_center[1])])
        run_adb_command(['shell', 'input', 'tap', str(send_message_center[0]), str(send_message_center[1])])
        SEND_MESSAGE = send_message_center
    except:
        run_adb_command(['shell', 'input', 'tap', str(SEND_MESSAGE[0]), str(SEND_MESSAGE[1])])
        run_adb_command(['shell', 'input', 'tap', str(SEND_MESSAGE[0]), str(SEND_MESSAGE[1])])
    # 生成消息并发送
    wait(1)
    for sentence in ai_reply.split('。'):
        if sentence and should_reply(sentence):
            input_text(sentence)
            wait(0.5)
            send_message_coords = find_text_coordinates('发送')
            if not send_message_coords:
                go_back()
                continue
            send_message_center = middle_center(send_message_coords)
            run_adb_command(['shell', 'input', 'tap', str(send_message_center[0]), str(send_message_center[1])])
            wait(0.5)
            # update_chat_records(customer['name'], sentence, 'me', time.strftime('%Y-%m-%d %H:%M:%S'))
    
    # 10. 记录到数据库
    # for record in parsed_records:
    #     update_chat_records(customer['name'], record['message'], record['sender'], record['time'])
    
    # 11. 返回
    go_back() #返回输入法
    go_back() #返回用户信息
    go_back() #返回到到通讯录页面
    processed_customers.add(customer['name'])

# 12. 检查是否有新的客户回复
check_new_replies()

# 关闭数据库连接
conn.close()
