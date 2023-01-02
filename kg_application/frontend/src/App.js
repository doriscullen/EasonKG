import './App.css';
import { Button, Form, Input, Row, Col, Typography } from 'antd';
import { useRef, useState } from 'react';
import Axios from 'axios';
function App() {
  const { Title, Paragraph, Text, Link } = Typography;
  let formRef = useRef()
  let [result, setResult] = useState("")
  let doQuery = async () => {
    let queryValue = formRef.current.getFieldValue()['query_value']
    let response = await Axios.get("api/query",
      {
        method: 'GET',
        params: {
          "query_value": queryValue
        }
      }
    )
    console.log(response);
    setResult(queryValue)
  }
  return (
    <>
      <Row>
        <Col span={10}></Col>
        <Col span={8}><Title >陈奕迅歌曲知识图谱</Title></Col>
        <Col span={6}></Col>
      </Row>
      <Row>
        <Col span={8}></Col>
        <Col span={8}>
          <Form
            name="basic"
            labelCol={{ span: 8 }}
            wrapperCol={{ span: 16 }}
            initialValues={{ remember: true }}
            autoComplete="off"
            layout='horizontal'
            id="form1"
            ref={formRef}
          >
            <Form.Item
              label="请输入你要查询的内容："
              name="query_value"
              rules={[{ required: true, message: '请在此输入你要查询的内容!' }]}
            >
              <Input />
            </Form.Item>

            <Form.Item wrapperCol={{ offset: 8, span: 16 }}>
              <Button type="primary" htmlType="submit" onClick={doQuery}>
                提交
              </Button>
            </Form.Item>
          </Form>
          <Text>查询结果: {result} </Text>
        </Col>
        <Col span={8}></Col>
      </Row>
    </>
  );
}

export default App;
