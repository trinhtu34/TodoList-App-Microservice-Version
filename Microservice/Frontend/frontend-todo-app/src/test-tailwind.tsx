// Test component để kiểm tra Tailwind CSS
export default function TestTailwind() {
  return (
    <div className="min-h-screen bg-gradient-to-r from-blue-500 to-purple-600 flex items-center justify-center">
      <div className="bg-white rounded-lg shadow-2xl p-8 max-w-md">
        <h1 className="text-4xl font-bold text-gray-900 mb-4">
          Tailwind CSS Works! ✅
        </h1>
        <p className="text-gray-600 mb-4">
          Nếu bạn thấy màu sắc và styling đẹp, Tailwind đã hoạt động.
        </p>
        <button className="w-full bg-indigo-600 text-white py-2 px-4 rounded-md hover:bg-indigo-700 transition-colors">
          Test Button
        </button>
      </div>
    </div>
  );
}
