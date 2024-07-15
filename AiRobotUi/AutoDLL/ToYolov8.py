from ultralytics import YOLO

# Load a model
model = YOLO('path/to/best')

# export the model to ONNX format
model.export(format='onnx')